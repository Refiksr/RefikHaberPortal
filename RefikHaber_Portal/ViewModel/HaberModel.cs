using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RefikHaber.ViewModels
{
    public class HaberModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [MaxLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        public string Baslik { get; set; }

        [MaxLength(5000, ErrorMessage = "Açıklama en fazla 5000 karakter olabilir.")]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Yazar adı zorunludur.")]
        [MaxLength(100, ErrorMessage = "Yazar adı en fazla 100 karakter olabilir.")]
        public string Yazar { get; set; }

        [Required(ErrorMessage = "Puanlama zorunludur.")]
        [Range(1, 10, ErrorMessage = "Puanlama 1 ile 10 arasında olmalıdır.")]
        public double Puan { get; set; }

        [MaxLength(500, ErrorMessage = "Resim URL'si en fazla 500 karakter olabilir.")]
        public string? ResimUrl { get; set; }

        [Required(ErrorMessage = "Kategori alanı zorunludur.")]
        public int KategoriId { get; set; }

        public List<SelectListItem> HaberTuruList { get; set; } = new List<SelectListItem>();
    }
}
