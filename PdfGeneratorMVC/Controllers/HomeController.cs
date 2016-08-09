using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Amazon.S3.Model;
using Amazon.SQS;
using Amazon.S3;
using Amazon.S3.Transfer;
using iTextSharp.text.pdf;
using Newtonsoft.Json.Linq;
using PdfGeneratorMVC.Models;

namespace PdfGeneratorMVC.Controllers
{
    public class HomeController : Controller
    {
        InformationModel im = new InformationModel();
        string dd;
        string mm;
        string yyyy;
        
        // GET: /Home/

        public ActionResult Index()
        {
            string url = "https://s3-us-west-1.amazonaws.com/www-medsymphony/templates/json/laborder-req.json";
            string data = GET(url);
            //Get Json Data From Url by newtonsoft json
            im.patient_id = JObject.Parse(data)["lab-order-req"]["patient"]["account-id"].ToString();
            im.patient_name = JObject.Parse(data)["lab-order-req"]["patient"]["name"]["first"] +" " + JObject.Parse(data)["lab-order-req"]["patient"]["name"]["last"];
            im.patient_gender = JObject.Parse(data)["lab-order-req"]["patient"]["gender"].ToString();
            im.patient_dob = JObject.Parse(data)["lab-order-req"]["patient"]["dob"].ToString();
            im.patient_street_address = JObject.Parse(data)["lab-order-req"]["patient"]["address"]["address1"].ToString();
            im.patient_city = JObject.Parse(data)["lab-order-req"]["patient"]["address"]["city"].ToString();
            im.patient_state = JObject.Parse(data)["lab-order-req"]["patient"]["address"]["state"].ToString();
            im.patient_zip = JObject.Parse(data)["lab-order-req"]["patient"]["address"]["zip"].ToString();
            im.patient_phone_mobile = JObject.Parse(data)["lab-order-req"]["patient"]["personal-phone"]["mobile"].ToString();
            string filename=FillForm();
            return Json(new
            {
                Status = "success",
                Url = LoadS3File(filename)
            }, JsonRequestBehavior.AllowGet);
            
           
        }

        //Get url data Method
        protected string GET(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                string data = reader.ReadToEnd();

                reader.Close();
                stream.Close();

                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        private string FillForm()// Write PDF text and other property
        {
            try
            {
            string pdfTemplate = Server.MapPath("~/PDF/lab_info.pdf");// local directory is here 
            string keyName = Server.MapPath("~/PDF/lab_info_" + im.patient_id + ".pdf");

            string filename = "lab_info_" + im.patient_id + ".pdf";

           
          
            PdfReader pdfReader = new PdfReader(pdfTemplate);
            PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(
                        keyName, FileMode.Create));

            AcroFields pdfFormFields = pdfStamper.AcroFields;

            //Split Date of Birth
            int i = 0;
            string[] words = im.patient_dob.Split('/');
            foreach (string s in words)
            {
                if (i == 0)
                {
                    mm = words[i];
                }
                if (i == 1)
                {
                    dd = words[i];
                }
                if (i == 2)
                {
                    yyyy = words[i];
                }
                
                i+=1;
            }

            // set form pdfFormFields
            // The first worksheet and fw4 form
            // Patient Information
            
                pdfFormFields.SetField("patient_name", im.patient_name);
                pdfFormFields.SetField("Day", dd);
                pdfFormFields.SetField("Month", mm);
                pdfFormFields.SetField("Year", yyyy);
                if (im.patient_gender == "Male")
                {
                    pdfFormFields.SetField("Male", "Yes");
                    pdfFormFields.SetField("Female", "0");
                }
                else
                {
                    pdfFormFields.SetField("Female", "Yes");
                    pdfFormFields.SetField("Male", "0");
                }


                pdfFormFields.SetField("patient_street_address", im.patient_street_address);
                string csz = im.patient_city + '_' + im.patient_state + '_' + im.patient_zip;
                pdfFormFields.SetField("city_state_zip", csz);
                pdfFormFields.SetField("phone", im.patient_phone_mobile);
                pdfFormFields.SetField("f1_04(0)", "8");
                pdfFormFields.SetField("f1_04(0)", "8");


                // flatten the form to remove editting options, set it to false
                // to leave the form open to subsequent manual edits
                pdfStamper.FormFlattening = false;

                // close the pdf
                pdfStamper.Close();

                // amazon s3 bucket save is here 
                return SaveFileToBucket(keyName, filename);
                
            }
            catch (Exception ex)
            {
                im.error = ex.Message;
                return im.error;
            }
        }

        //Method for Uploading file to bucket 
        private string SaveFileToBucket(string keyName, string filename)
        {
            string accessKeyId = PdfGeneratorMVC.Properties.Settings.Default.accessKeyId;
            string secretAccessKey = PdfGeneratorMVC.Properties.Settings.Default.secretAccessKey;
            string bucketName = PdfGeneratorMVC.Properties.Settings.Default.bucketName;
            using (IAmazonS3 client = new AmazonS3Client(accessKeyId, secretAccessKey, Amazon.RegionEndpoint.USEast1))
            {

                using (TransferUtility fileTransferUtility = new TransferUtility(client))
                {
                    fileTransferUtility.Upload(keyName, bucketName);
                }
            }

            return filename;
        }

        // After uploading written pdf method generate a url link function
        //Load file url after save file to bucket
        public string LoadS3File(string fileName)
        {
            string accessKeyId = PdfGeneratorMVC.Properties.Settings.Default.accessKeyId;
            string secretAccessKey = PdfGeneratorMVC.Properties.Settings.Default.secretAccessKey;
            string bucketName = PdfGeneratorMVC.Properties.Settings.Default.bucketName;
            string key = Path.GetFileName(fileName);

            string url = string.Empty;
            using (IAmazonS3 client = new AmazonS3Client(accessKeyId, secretAccessKey, Amazon.RegionEndpoint.USEast1))
            {
                using (TransferUtility fileTransferUtility = new TransferUtility(client))
                {
                    GetPreSignedUrlRequest request = new GetPreSignedUrlRequest();
                    request.BucketName = bucketName;
                    request.Key = key;
                    request.Expires = DateTime.Now.AddDays(1);
                    request.Protocol = Protocol.HTTPS;
                    url = client.GetPreSignedURL(request);
                }
            }
            return url;
        }
    }
}
