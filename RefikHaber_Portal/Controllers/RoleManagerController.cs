using Microsoft.AspNetCore.Mvc;
using RefikHaber.Models;
using RefikHaber_Portal.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace RefikHaber_Portal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleManagerController : Controller
    {
        private readonly RoleManagerRepository _roleManagerRepository;

        public RoleManagerController(RoleManagerRepository roleManagerRepository)
        {
            _roleManagerRepository = roleManagerRepository;
        }

        public async Task<IActionResult> GetRoleList()
        {
            var roles = await _roleManagerRepository.GetAllRolesAsync();
            return View(roles.ToList());
        }

        public IActionResult RoleAdd()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleAdd(AppRole role)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManagerRepository.AddRoleAsync(role);
                if (result.Succeeded)
                {
                    TempData["deleteRoleSuccess"] = "Rol eklendi!";
                    return RedirectToAction("GetRoleList");
                }
                else
                {
                    ModelState.AddModelError("", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> RoleDelete(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return NotFound("Rol ID'si bulunamadı.");
            }

            if (await _roleManagerRepository.HasUsersInRoleAsync(roleId))
            {
                TempData["deleteRoleError"] = "Bu role sahip en az bir kullanici varoldugu icin rol silinemedi!";
                return RedirectToAction("GetRoleList");
            }

            var result = await _roleManagerRepository.DeleteRoleAsync(roleId);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Rol başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Rol silinirken bir hata oluştu.";
            }

            return RedirectToAction("GetRoleList");
        }


        public IActionResult GetUserList()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserListAjax()
        {
            var users = await _roleManagerRepository.GetUsersAsync();
            return Json(users);
        }

       public async Task<IActionResult> ManageUserRoles()
    {
        try
        {
            var users = await _roleManagerRepository.GetAllUsersAsync();
            var userRoles = await _roleManagerRepository.GetUserRolesGroupedAsync();
            var roles = await _roleManagerRepository.GetAllRolesAsync();

            if (userRoles == null)
            {
                userRoles = new Dictionary<string, List<string>>();
            }

            // Boş kullanıcı ID'leri için boş rol listesi oluştur
            foreach (var user in users)
            {
                if (!userRoles.ContainsKey(user.Id))
                {
                    userRoles[user.Id] = new List<string>();
                }
            }

            ViewBag.UserRoles = userRoles;
            ViewBag.Roles = roles;

            return View(users);
        }
        catch (Exception ex)
        {
            // Hata durumunda loglama yapabilirsiniz
            ModelState.AddModelError("", "Kullanıcı rolleri yüklenirken bir hata oluştu.");
            return View(new List<ApplicationUser>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageUserRoles(string userId, string[] selectedRoles)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Kullanıcı ID'si geçersiz.");
                return RedirectToAction(nameof(ManageUserRoles));
            }

            if (selectedRoles == null || selectedRoles.Length == 0)
            {
                ModelState.AddModelError("", "Lütfen en az bir rol seçiniz.");
                return RedirectToAction(nameof(ManageUserRoles));
            }

            await _roleManagerRepository.UpdateUserRolesAsync(userId, selectedRoles);
            TempData["SuccessMessage"] = "Kullanıcı rolleri başarıyla güncellendi.";
            
            return RedirectToAction(nameof(ManageUserRoles));
        }
        catch (Exception ex)
        {
            // Hata durumunda loglama yapabilirsiniz
            TempData["ErrorMessage"] = "Roller güncellenirken bir hata oluştu.";
            return RedirectToAction(nameof(ManageUserRoles));
        }
    }

        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            var users = await _roleManagerRepository.GetAllUsersAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("Kullanıcı ID'si bulunamadı.");
            }

            var result = await _roleManagerRepository.DeleteUserAsync(userId);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanıcı silinirken bir hata oluştu.";
            }

            return RedirectToAction("UserList");
        }
    }
}
