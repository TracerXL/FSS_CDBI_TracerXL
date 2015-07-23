using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace FSSCDBI_5
{
    class Program
    {

        // use this data context for all API relatecd GET actions
        private FSSCDBI_Entities db = new FSSCDBI_Entities();

        public static HashSet<Hashtable> jsonObjects;

        static void Main(string[] args)
        {

            string _apiUrlRoot = "https://run.journeyapps.com/api/v1/";
            var apiPath = String.Format("{0}/{1}.{2}", _apiUrlRoot, "site", "json");
            var client = CreateWebClient();
            var response = client.DownloadString(apiPath);
            var result = JsonConvert.DeserializeObject<List<jny_site>>(response);
            //result.Reverse();
            logger("Site Count = " + result.Count.ToString());

            foreach (var site in result)
            {
                if (site.id != null)
                {
                    var db = new FSSCDBI_Entities();
                    if (db.jny_site.Any(s => s.id == site.id))
                    {
                        logger(site.id + " Imported ");

                        continue;
                    }
                    // check if site is concluded, 0 = 'yes', 1 = 'no'
                    if (site.is_complete == 1)
                    {
                        logger(site.id + " In_Progress ");
                    }
                    else
                    {

                        if (!site.worker_id.HasValue)
                        {
                            logger(site.id + " Not_Allocated ");
                            continue;
                        }

                        string id = site.id.ToString();
                        //Guid Newsiteid = Guid.TryParse(site.id);
                        bool allPhotosSynced = AllPhotosSynced(id);

                        if (allPhotosSynced)
                        {
                            logger(site.id + " Ready_For_Import ");
                            logger(site.id + " " + site.display_name + " Importing ");
                            SaveApiSite(site.id);
                            continue;
                        }
                        else
                        {
                            logger(site.id + " Not_Fully_Synced ");
                            continue;
                        }
                    }

                }

            }
            logger(" Update Done ");
            Console.ReadLine();
        }

        public static bool SaveApiSite(Guid? id)
        {
            var db = new FSSCDBI_Entities();

            if (id == null) { return false; }
            //json_serializer = new JavaScriptSerializer();
            jsonObjects = new HashSet<Hashtable>();

            // initialise RESTful API Data Context
            //InitializeApiRepositories(instanceId);
            string sids = id.ToString();
            var site = JsonConvert.DeserializeObject<jny_site>(GetById(sids, "site"));
            var rawSite = GetById(sids, "site");

            jsonObjects = GetObjectsFromJSON(rawSite, "gps");
            if (jsonObjects.ToList().Count > 0)
            {
                site.latitude = jsonObjects.ToList()[0]["latitude"].ToString();
                site.longitude = jsonObjects.ToList()[0]["longitude"].ToString();
                site.altitude = jsonObjects.ToList()[0]["altitude"] != null ? jsonObjects.ToList()[0]["altitude"].ToString() : "";
                site.horizontal_accuracy = jsonObjects.ToList()[0]["horizontal_accuracy"] != null ? jsonObjects.ToList()[0]["horizontal_accuracy"].ToString() : "";
            }

            db.jny_site.Add(site);
            logger(site.id + " Site Data Recieved ");


            if (site.worker_id != null)
            {
                string widw = site.worker_id.ToString();
                //var worker = GetById(widw,"workers");
                var worker = JsonConvert.DeserializeObject<jny_worker>(GetById(widw, "workers"));
                db.jny_worker.Add(worker);
            }
            logger(site.id + " Worker Data Recieved ");

            var generals = Get("general", "q[site_id]=" + site.id);
            var generalsList = JsonConvert.DeserializeObject<List<jny_general>>(generals);
            //var result = JsonConvert.DeserializeObject<List<jny_site>>(response);
            if (generalsList.Count > 0)
            {
                foreach (var g in generalsList)
                {
                    db.jny_general.Add(g);
                }
            }
            logger(site.id + " General Data Recieved " + generalsList.Count.ToString());

            var areas = Get("area", "q[site_id]=" + site.id);
            var areasList = JsonConvert.DeserializeObject<List<jny_area>>(areas);

            if (areasList.Count > 0)
            {
                foreach (var a in areasList)
                {
                    db.jny_area.Add(a);
                }
            }
            logger(site.id + " Area Data Recieved " + areasList.Count.ToString());

            var subAreas = Get("sub_area", "q[site_id]=" + site.id);
            var subAreasList = JsonConvert.DeserializeObject<List<jny_sub_area>>(subAreas);

            if (subAreasList.Count > 0)
            {
                foreach (var s in subAreasList)
                {
                    db.jny_sub_area.Add(s);
                }
            }
            logger(site.id + " Sub Area Data Recieved " + subAreasList.Count.ToString());

            var elementgroups = Get("element_group", "q[site_id]=" + site.id);
            var elementGroupsList = JsonConvert.DeserializeObject<List<jny_element_group>>(elementgroups);

            if (elementGroupsList.Count > 0)
            {
                foreach (var eg in elementGroupsList)
                {
                    db.jny_element_group.Add(eg);
                }
            }
            logger(site.id + " Element Group Data Recieved " + elementGroupsList.Count.ToString());

            var elements = Get("element", "q[site_id]=" + site.id);
            var elementsList = JsonConvert.DeserializeObject<List<jny_element>>(elements);

            if (elementsList.Count > 0)
            {
                foreach (var e in elementsList)
                {
                    db.jny_element.Add(e);
                }
            }
            logger(site.id + " Element Data Recieved " + elementsList.Count.ToString());

            var materials = Get("material", "q[site_id]=" + site.id);
            var materialsList = JsonConvert.DeserializeObject<List<jny_material>>(materials);

            if (materialsList.Count > 0)
            {
                foreach (var m in materialsList)
                {
                    db.jny_material.Add(m);
                }
            }
            logger(site.id + " Material Data Recieved " + materialsList.Count.ToString());

            var conditions = Get("condition", "q[site_id]=" + site.id);
            var conditionsList = JsonConvert.DeserializeObject<List<jny_condition>>(conditions);

            if (conditionsList.Count > 0)
            {
                foreach (var cz in conditionsList)
                {
                    db.jny_condition.Add(cz);
                }
            }
            logger(site.id + " Condition Data Recieved " + conditionsList.Count.ToString());

            logger(site.id + " Recieveing Photos ");
            var photos = Get("photo_placeholder", "q[sync_site_id]=" + site.id);
            var photosList = JsonConvert.DeserializeObject<List<jny_photo_placeholder>>(photos);

            if (photosList.Count > 0)
            {
                foreach (var cpz in photosList)
                {
                    var photo = new jny_photo_placeholder { };
                    string pids = cpz.id.ToString();
                    var rawPhoto = GetById(pids, "photo_placeholder");
                    var obj = GetObjectsFromJSON(rawPhoto, "gps");
                    var pic = GetObjectsFromJSON(rawPhoto, "picture");

                    photo._updated_at = cpz._updated_at;
                    photo.area_id = cpz.area_id;
                    photo.condition_id = cpz.condition_id;
                    photo.element_id = cpz.element_id;
                    photo.material_id = cpz.material_id;
                    photo.site_id = cpz.site_id;
                    photo.sub_area_id = cpz.sub_area_id;
                    photo.sync_site_id = cpz.sync_site_id;
                    photo.id = cpz.id;
                    photo.display_name = cpz.display_name;

                    //photo.latitude = obj.ToList()[0]["latitude"].ToString();
                    //photo.longitude = obj.ToList()[0]["longitude"].ToString();
                    //photo.altitude = obj.ToList()[0]["altitude"] != null ? obj.ToList()[0]["altitude"].ToString() : "";
                    //photo.horizontal_accuracy = obj.ToList()[0]["horizontal_accuracy"] != null ? obj.ToList()[0]["horizontal_accuracy"].ToString() : "";

                    photo.latitude = obj.ToList()[0]["latitude"] != null ? obj.ToList()[0]["latitude"].ToString() : "";
                    photo.longitude = obj.ToList()[0]["longitude"] != null ? obj.ToList()[0]["longitude"].ToString() : "";
                    photo.altitude = obj.ToList()[0]["altitude"] != null ? obj.ToList()[0]["altitude"].ToString() : "";
                    photo.horizontal_accuracy = obj.ToList()[0]["horizontal_accuracy"] != null ? obj.ToList()[0]["horizontal_accuracy"].ToString() : "";
                    photo.vertical_accuracy = obj.ToList()[0]["vertical_accuracy"] != null ? obj.ToList()[0]["vertical_accuracy"].ToString() : "";

                    photo.image_data = pic.ToList()[0]["fullscreen"] != null ? GetImageData(pic.ToList()[0]["fullscreen"].ToString()) : "";
                    photo.thumbnail = pic.ToList()[0]["thumbnail"] != null ? pic.ToList()[0]["thumbnail"].ToString() : "";
                    photo.fullscreen = pic.ToList()[0]["fullscreen"] != null ? pic.ToList()[0]["fullscreen"].ToString() : "";
                    photo.original = pic.ToList()[0]["original"] != null ? pic.ToList()[0]["original"].ToString().Replace("}", "").Replace("]", "").Trim() : "";

                    db.jny_photo_placeholder.Add(photo);
                }
            }

            logger(site.id + " Photo Data Recieved " + photosList.Count.ToString());

            var siteInDb = db.jny_site.FirstOrDefault(s => s.id == site.id);
            bool existsInDb = siteInDb != null;
            if (!existsInDb)
            {

                if (generalsList.Count > 0)
                {
                    foreach (var g in generalsList)
                    {
                        db.jny_general.Add(g);
                    }
                }

                if (areasList.Count > 0)
                {
                    foreach (var a in areasList)
                    {
                        db.jny_area.Add(a);
                    }
                }

                if (subAreasList.Count > 0)
                {
                    foreach (var s in subAreasList)
                    {
                        db.jny_sub_area.Add(s);
                    }
                }

                if (elementGroupsList.Count > 0)
                {
                    foreach (var eg in elementGroupsList)
                    {
                        db.jny_element_group.Add(eg);
                    }
                }

                if (elementsList.Count > 0)
                {
                    foreach (var e in elementsList)
                    {
                        db.jny_element.Add(e);
                    }
                }

                if (materialsList.Count > 0)
                {
                    foreach (var m in materialsList)
                    {
                        db.jny_material.Add(m);
                    }
                }

                if (conditionsList.Count > 0)
                {
                    foreach (var c in conditionsList)
                    {
                        db.jny_condition.Add(c);
                    }
                }

                db.SaveChanges();
                logger(site.id + " Data Saved to SQL Server ");
                //return site;
            }

            return true;

        }

        public static bool logger(string str)
        {
            using (StreamWriter file = new StreamWriter("FSSCDBI_Logger.txt", true))
            {
                file.WriteLine(str);
            }
            Console.WriteLine(str);
            return false;
        }

        static WebClient CreateWebClient()
        {
            var webClient = new WebClient();
            var creds = "54e5b1170278944c410005dc:gnQmKavgWb1v_AME1MK9";
            var bcreds = Encoding.ASCII.GetBytes(creds);
            var base64Creds = Convert.ToBase64String(bcreds);
            webClient.Headers.Add("Authorization", "Basic " + base64Creds);
            return webClient;
        }

        public static bool AllPhotosSynced(string syncSiteId)
        {
            string _apiUrlRoot = "https://run.journeyapps.com/api/v1/";
            var apiPath = String.Format("{0}/{1}.{2}?{3}", _apiUrlRoot, "photo_placeholder", "json", "q[sync_site_id]=" + syncSiteId + "&q[fullscreen.null]=true");
            var client = CreateWebClient();
            var response = client.DownloadString(apiPath);

            jsonObjects = new HashSet<Hashtable>();
            jsonObjects = GetObjectsFromJSON(response, "picture");

            return jsonObjects.Count > 0;
        }

        public static string GetImageData(string mediaUrl)
        {
            mediaUrl = mediaUrl.Replace("}", "").Replace("]", "").Trim();
            //return Convert.FromBase64String(apiSiteContext.Get("media", mediaUrl));
            return Get("media", mediaUrl);
        }

        public static string GetById(string id, string _entityType)
        {
            string _apiUrlRoot = "https://run.journeyapps.com/api/v1/";

            var apiPath = String.Format("{0}/{1}/{2}.{3}", _apiUrlRoot, _entityType.ToLower(), id, "json");

            var client = CreateWebClient();
            var response = client.DownloadString(apiPath);

            return response;
        }

        public static string Get(string type, string q)
        {
            string _apiUrlRoot = "https://run.journeyapps.com/api/v1/";
            string apiPath = String.Empty;

            if (type.ToLower().Trim() != "media")
            {

                apiPath = String.Format("{0}/{1}.{2}?{3}", _apiUrlRoot, type, "json", q);

                var client = CreateWebClient();
                var response = client.DownloadString(apiPath);

                return response;

            }
            else
            {
                string base64String;

                apiPath = String.Format("{0}{1}", _apiUrlRoot.Substring(0, _apiUrlRoot.IndexOf("/api")), q);

                var client = CreateWebClient();

                byte[] responseimage = client.DownloadData(apiPath);

                base64String = Convert.ToBase64String(responseimage);

                return base64String;
            }
        }

        public static HashSet<Hashtable> GetObjectsFromJSON(string response, string objectName)
        {
            var jsonSet = new HashSet<Hashtable>();
            //string[] rawProperties = response.Split('{')[1].Split(',');
            string[] rawProperties = response.Split('{');

            for (int a = 0; a < rawProperties.Length; a++)
            {
                if (rawProperties[a].Contains(objectName))
                {
                    if (rawProperties.Length - a > 1)
                    {
                        var tempHashtable = new Hashtable();
                        string[] gpsProperties = rawProperties[a + 1].Split(',');
                        for (int b = 0; b < gpsProperties.Length; b++)
                        {
                            if (gpsProperties[b].IndexOf(':') == -1) break;
                            var key =
                                gpsProperties[b].Split(':')[0].Replace("\n", "")
                                    .Replace("\\", "")
                                    .Replace("\"", "")
                                    .Trim();
                            var value =
                                gpsProperties[b].Split(':')[1].Replace("\n", "")
                                    .Replace("\\", "")
                                    .Replace("\"", "")
                                    .Trim();
                            tempHashtable[key] = value.Contains("null") ? null : value;
                        }
                        if (tempHashtable.Count > 0)
                            jsonSet.Add(tempHashtable);
                    }
                }
            }
            return jsonSet;
        }
    }
}
