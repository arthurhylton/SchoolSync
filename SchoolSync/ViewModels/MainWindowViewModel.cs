using SchoolSync.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SchoolSync
{
    class MainWindowViewModel : ModelBase
    {
        public Dictionary<string, string> Parishes;
        public Dictionary<string, string> Regions;
        public List<School> OldSchools { get; set; }
        public List<School> NewSchools { get; set; }
        public ObservableCollection<School> Changes { get; set; }
        public ObservableCollection<School> Errors { get; set; }
        public string OldFilePath { get; set; }
        public string OldFileDirectory { get; set; }
        public string NewFilePath { get; set; }
        public string NewFileDirectory { get; set; }

        //string oldFfileName = "CENSUSo1.DBF";
        //string newFileName = "CENSUSn.DBF";
        public string oldFfileName = "";
        public string newFileName = "";

        public MainWindowViewModel()
        {
            NewSchools = new List<School>();
            OldSchools = new List<School>();

            Changes = new ObservableCollection<School>();
            Errors = new ObservableCollection<School>();

            Regions = new Dictionary<string, string>();
            Regions.Add("1", "Kingston");
            Regions.Add("2", "Port Antonio");
            Regions.Add("3", "Brown's Town");
            Regions.Add("4", "Montego Bay");
            Regions.Add("5", "Mandeville");
            Regions.Add("6", "Old Harbour");

            Parishes = new Dictionary<string, string>();
            Parishes.Add("1", "Kingston");
            Parishes.Add("2", "St. Andrew");
            Parishes.Add("3", "St. Thomas");
            Parishes.Add("4", "Portland");
            Parishes.Add("5", "St. Mary");
            Parishes.Add("6", "St. Ann");
            Parishes.Add("7", "Trelawny");
            Parishes.Add("8", "St. James");
            Parishes.Add("9", "Hanover");
            Parishes.Add("10", "Westmoreland");
            Parishes.Add("11", "St. Elizabeth");
            Parishes.Add("12", "Manchester");
            Parishes.Add("13", "Clarendon");
            Parishes.Add("14", "St. Catherine");
        }

        public void ProcessFiles()
        {
            if (string.IsNullOrWhiteSpace(NewFilePath))
            {
                MessageBox.Show("Please choose a \"New file\"");
                return;
            }
            NewFileDirectory = Path.GetDirectoryName(NewFilePath);
            NewSchools.Clear();// = new List<School>();
            OldSchools.Clear();// = new List<School>();

            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + NewFileDirectory + ";Extended Properties=dBASE IV;User ID=Admin;Password=;";
            using (OleDbConnection con = new OleDbConnection(constr))
            {
                var sql = "select * from " + newFileName;
                OleDbCommand cmd = new OleDbCommand(sql, con);
                con.Open();
                DataSet ds = new DataSet(); ;
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(ds);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    NewSchools.Add(new School { SchoolCode = row[0].ToString().Trim(), Name = row[2].ToString().Trim(), Gender = row[51].ToString(), RegionNumber = row[79].ToString().Split('.')[0].Trim(), RegionName = row[78].ToString().Trim(), ParishNumber = row[77].ToString().Split('.')[0].Trim(), ParishName = row[76].ToString().Trim(), SchoolTypeCode = row[83].ToString().Split(' ')[0].Trim(), SchoolTypeName = row[82].ToString().Trim() });
                }
            }

            //get old school data (from the last time a sync was performed)
            //if (File.Exists("C:\\Users\\Arthur\\Desktop\\temp-working-stuff\\" + oldFfileName))
            if (File.Exists(OldFilePath))
            {
                OldFileDirectory = Path.GetDirectoryName(OldFilePath);
                constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + OldFileDirectory + ";Extended Properties=dBASE IV;User ID=Admin;Password=;";
                using (OleDbConnection con = new OleDbConnection(constr))
                {
                    var sql = "select * from " + oldFfileName;
                    OleDbCommand cmd = new OleDbCommand(sql, con);
                    DataSet ds = new DataSet(); ;
                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(ds);
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        OldSchools.Add(new School { SchoolCode = row[0].ToString().Trim(), Name = row[2].ToString().Trim(), Gender = row[51].ToString(), RegionNumber = row[79].ToString().Split('.')[0].Trim(), RegionName = row[78].ToString().Trim(), ParishNumber = row[77].ToString().Split('.')[0].Trim(), ParishName = row[76].ToString().Trim(), SchoolTypeCode = row[83].ToString().Split(' ')[0].Trim(), SchoolTypeName = row[82].ToString().Trim() });
                    }
                }
            }
            GetChangedSchools(OldSchools, NewSchools);
            GetSchoolErrors(NewSchools);
            RaisePropertyChanged("Changes");
            RaisePropertyChanged("Errors");
            //Changes = GetChangedSchools(OldSchools, NewSchools);
            //Errors = GetSchoolErrors(NewSchools);
        }

        private void GetChangedSchools(List<School> oldSchools, List<School> newSchools)
        {
            Changes.Clear();
            //ObservableCollection<School> changedSchools = new ObservableCollection<School>();
            foreach (var newSchool in newSchools)
            {
                var oldTmp = oldSchools.FirstOrDefault(o => o.SchoolCode == newSchool.SchoolCode);
                if (oldTmp == null || oldTmp.Implode() != newSchool.Implode())
                {
                    Changes.Add(newSchool);
                }
            }
            //return changedSchools;
        }

        private void GetSchoolErrors(List<School> newSchools)
        {
            Errors.Clear();
            //ObservableCollection<School> errorSchools = new ObservableCollection<School>();
            string tmpRegionName;
            foreach (var newSchool in newSchools)
            {
                //Ensure that region name and number align
                if (Regions.TryGetValue(newSchool.RegionNumber, out tmpRegionName))
                {
                    if (newSchool.RegionName != tmpRegionName)
                    {
                        newSchool.ErrorMessage += "Region name does not match region number. ";
                        Errors.Add(newSchool);
                    }
                }
                else
                {
                    newSchool.ErrorMessage += "Region number not found. ";
                    Errors.Add(newSchool);
                }

                string tmpParishName;
                //Ensure that parish name and number align
                if (Parishes.TryGetValue(newSchool.ParishNumber, out tmpParishName))
                {
                    if (newSchool.ParishName != tmpParishName)
                    {
                        newSchool.ErrorMessage += "Parish name does not match parish number. ";
                        Errors.Add(newSchool);
                    }
                }
                else
                {
                    newSchool.ErrorMessage += "Parish number not found. ";
                    Errors.Add(newSchool);
                }
            }
            //return errorSchools;
        }

        public string GetUpdateSQL()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var newSchool in NewSchools)
            {
                stringBuilder.AppendLine(string.Format("INSERT INTO all_schools (school_code,name,parish_number,parish_name,region_number,region_name,type_code,type_name,gender) VALUES ( '{0}','{1}',{2},'{3}',{4},'{5}','{6}','{7}','{8}' ) ON DUPLICATE KEY UPDATE type_code = VALUES(type_code), type_name = VALUES(type_name), gender = VALUES(gender);", newSchool.SchoolCode, newSchool.Name.Replace("'","\\'"), newSchool.ParishNumber, newSchool.ParishName.Replace("'", "\\'"), newSchool.RegionNumber, newSchool.RegionName.Replace("'", "\\'"), newSchool.SchoolTypeCode, newSchool.SchoolTypeName.Replace("'", "\\'"), newSchool.Gender) + Environment.NewLine);
            }
            return stringBuilder.ToString();
        }
    }
}
