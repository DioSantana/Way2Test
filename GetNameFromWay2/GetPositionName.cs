using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Net;
using System.Web;

namespace GetNameFromWay2
{
    public class GetPositionName
    {
        private DataSet dsWay2 = new DataSet();

        private DataTable TableNames = new DataTable();

        private string lFile = @Directory.GetCurrentDirectory() + "\\lstNames.xml";
        private string lFileXSD = @Directory.GetCurrentDirectory() + "\\lstNamesXSD.xml";

        HttpWebRequest request = null;

        HttpWebResponse response = null;

        string url = "http://teste.way2.com.br/dic/api/words/";

            string lName;
            int lCat = 0;

        public GetPositionName()
        {
            if (File.Exists(lFile))
            {
                dsWay2.ReadXmlSchema(lFileXSD);
                dsWay2.ReadXml(lFile);
                if (dsWay2.Tables[0].Rows.Count == 0)
                {
                    TableNames.TableName = "Names";

                    DataColumn colCodigo = new DataColumn("Codigo");
                    colCodigo.DataType = System.Type.GetType("System.UInt64");
                    TableNames.Columns.Add(colCodigo);

                    DataColumn colNome = new DataColumn("Nome");
                    colNome.DataType = System.Type.GetType("System.String");
                    TableNames.Columns.Add(colNome);

                    TableNames.PrimaryKey = new DataColumn[] { colNome };

                    dsWay2.Tables.Add(TableNames);
                }
                else
                {
                    TableNames = dsWay2.Tables[0];
                }
            }
            else
            {
                TableNames.TableName = "Names";

                DataColumn colCodigo = new DataColumn("Codigo");
                colCodigo.DataType = System.Type.GetType("System.UInt64");
                TableNames.Columns.Add(colCodigo);

                DataColumn colNome = new DataColumn("Nome");
                colNome.DataType = System.Type.GetType("System.String");
                TableNames.Columns.Add(colNome);

                TableNames.PrimaryKey = new DataColumn[] { colNome };

                dsWay2.Tables.Add(TableNames);
            }
        }

        public void Close()
        {
            dsWay2.WriteXml(lFile);
            dsWay2.WriteXmlSchema(lFileXSD);
        }

        public Int64[] searchName(string pName)
        {
            Int64[] lPos = new Int64[2]{0,0};
            DataRow lRow = TableNames.Rows.Find(pName);
            lCat = 0;

            if (lRow != null)
                lPos[0] = Convert.ToInt64(lRow[0].ToString());
            else
                lPos[0] = -1;
           

            if (lPos[0] == -1)
            {
                lPos = SearchNewName(pName,lPos);
            }
            else
            {
                lPos = SearchExistName(pName, lPos);
            }

            return lPos;
        }

        private Int64[] SearchExistName(string pName, Int64[] pPos)
        {
            request = WebRequest.Create(url + pPos[0].ToString()) as HttpWebRequest;

            lCat++;

            using (response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                lName = reader.ReadToEnd();

                int lRet = pName.ToUpper().CompareTo(lName.ToUpper());

                if (lRet < 0)
                {
                    pPos = SearchNewName(pName, pPos);
                }
            }

            return pPos;
        }

        private Int64[] SearchNewName(string pName, Int64[] pPos)
        {
            long lNumber = 1000;
            long lMinNumber = 0;
            long lMaxNumber =0;

            bool Stop = false;
            bool Adjust = false;

            while (!Stop)
            {

                request = WebRequest.Create(url + lNumber) as HttpWebRequest;

                using (response = request.GetResponse() as HttpWebResponse)
                {
                    if ((lNumber-lMinNumber) == 0)
                    {
                        pPos[0] = -1;
                        pPos[1] = lCat;
                    
                        Stop = true;

                        continue;
                    }

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        lCat++;

                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        lName = reader.ReadToEnd();

                        int lRet = pName.ToUpper().CompareTo(lName.ToUpper());

                        if (lRet < 0)
                        {
                            if (Adjust)
                            {
                                lMaxNumber = lNumber;
                                lNumber = lMinNumber + ((lMaxNumber - lMinNumber) / 2); 
                            }
                            else
                            {
                                lMaxNumber = lNumber;
                                lNumber = lMinNumber + ((lMaxNumber - lMinNumber) / 2);
                                Adjust = true;
                            }
                        }
                        else if (lRet > 0)
                        {
                            if (Adjust)
                            {
                                lMinNumber = lNumber;
                                lNumber = lNumber+((lMaxNumber - lMinNumber) / 2);
                            }
                            else
                            {
                                lMinNumber = lNumber;

                                lNumber = lNumber * (lCat + 1);
                            }
                        }
                        else
                        {
                            pPos[0] = lNumber;
                            pPos[1] = lCat;

                            TableNames.Rows.Add(pPos[0], pName);

                            Stop = true;

                            continue;
                        }

                    }
                    else
                    {
                        pPos[0] = -1;
                        pPos[1] = -1;
                    
                        Stop = true;

                        continue;
                    }
                }
            }
            return pPos;
        }

    }
}
