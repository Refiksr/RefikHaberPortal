using System.ComponentModel.DataAnnotations;

namespace RefikHaber.ViewModels
{
    public class HaberTuruModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori Adı Boş Bırakılamaz!")]
        [MaxLength(25)]
        public string Ad { get; set; }
    }
}
