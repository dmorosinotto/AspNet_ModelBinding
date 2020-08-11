
using System.ComponentModel.DataAnnotations;
using JurisTempus.Data.Entities;

namespace JurisTempus.ViewModels
{
    public class CaseDTO
    {
        //MAPPING SEMPLICE AUTOMATICO NOMI CORRISPONDENTI --> FUNZIONA ANCHE X COLLECTION 
        public int Id { get; set; }

        // [Required]
        // [MinLength(9)]
        // [MaxLength(50)]
        public string FileNumber { get; set; }
        // ESEMPI DI VALIDAZIONE TRAMITE ATTRIBUTI
        // [Required]
        public string Status { get; set; } // MAPPATURA AUTOMATICA DA ENUM -> string
        public int StatusId { get; set; } // MAPPATURA CON REGOLA DA ENUM -> int valore
    }
}
