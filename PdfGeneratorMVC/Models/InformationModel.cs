using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PdfGeneratorMVC.Models
{
    public class InformationModel
    {
        public string error { get; set; }
        public string patient_id { get; set; }
        public string patient_guid { get; set; }
        public string patient_name { get; set; }
        public string patient_dob { get; set; }
        public string patient_gender { get; set; }
        public string patient_street_address { get; set; }
        public string patient_city { get; set; }
        public string patient_state { get; set; }
        public string patient_zip { get; set; }

        public string ethnicity { get; set; }
        public string preferred_language { get; set; }
        public string patient_phone_home { get; set; }
        public string patient_phone_mobile { get; set; }
        public string patient_phone_work { get; set; }
        public string patient_phone_fax { get; set; }
        public string patient_insurance { get; set; }

        public string eprescriber_guid { get; set; }
        public string eprescriber_name { get; set; }
        public string eprescriber_title { get; set; }
        public string eprescriber_email { get; set; }
        public string eprescriber_address { get; set; }
        public string eprescriber_city { get; set; }
        public string eprescriber_state { get; set; }
        public string eprescriber_zip { get; set; }
        public string eprescriber_phone_home { get; set; }
        public string eprescriber_phone_mobile { get; set; }
        public string eprescriber_phone_fax { get; set; }
        public string eprescriber_phone_work { get; set; }
        public string eprescriber_npi { get; set; }
        public string eprescriber_dea_id { get; set; }
        public string eprescriber_dea_state { get; set; }
        public string eprescriber_signature { get; set; }

        public string eprescriber_loinc_code { get; set; }
        public string eprescriber_loinc_name { get; set; }
        public string eprescriber_loinc_long_name { get; set; }

        public string lab_id { get; set; }
        public string lab_requested { get; set; }
        public string lab_accepted { get; set; }
        public string lab_cpt { get; set; }
        public string lab_icd { get; set; }
        public string lab_status { get; set; }
        public string lab_webhook { get; set; }

        public string api_key { get; set; }
        public string api_secret { get; set; }
        public string auth_token { get; set; }
    }
}