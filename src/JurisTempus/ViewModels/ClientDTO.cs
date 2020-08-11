
using System.Collections.Generic;

namespace JurisTempus.ViewModels
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; } // NOME PROP DIVERSO
        public string Phone { get; set; }

        public string FullAddress { get; set; } // PROP CALCOLATA CON CONCATENAZIONE Address1+2+3
        public string Address1 { get; set; } // PROP SPECIFICA FLAT A MANO NON BY CONVENTION!

        // AUTOMAPPER FLAT AUTOMATICO PREFISSO Address = NOME ENTITY -> Address<NOMEPROP>
        public string AddressCityTown { get; set; }
        public string AddressStateProvince { get; set; }
        public string AddressPostalCode { get; set; }
        public string AddressCountry { get; set; }

        // MAPPING DI COLLECTION - RELAZIONI ALTRE ENTITY -> DTO
        public ICollection<CaseDTO> Cases { get; set; }

    }
}
