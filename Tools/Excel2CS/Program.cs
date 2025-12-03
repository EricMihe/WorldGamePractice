using System;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace Excel2CS
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args!=null&&args.Length>=3)
            {
                string excelDirectory = args[0];//配置表的路径
                string outputDirectory = args[1];//输出CS的路径

                path = excelDirectory;
                writePath = outputDirectory;
                jsonPath = args[2];
            }
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            fileDct.Clear();
            Start();
        }

        static string path = AppDomain.CurrentDomain.BaseDirectory + "/../../../../../Tools/Excel/";
        static string writePath = AppDomain.CurrentDomain.BaseDirectory + "/../../../../../Tools/GameConfig/";
        static string jsonPath = writePath + "/../" + "JsonFile";
        static List<string> excelList = new List<string>();

        public static void GetAllFiles(string path,ref List<string> lst)
        {
            lst.AddRange(Directory.GetFiles(path));
            string[] dir = Directory.GetDirectories(path);
            for (int i = 0; i < dir.Length; i++)
            {
                GetAllFiles(dir[i],ref lst);
            }
        }

        //扫描目录下的所有Excel文件
        static void Start()
        {
            //https://github.com/ExcelDataReader/ExcelDataReader#important-note-on-net-core
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            List<string> fileLst = new List<string>();
            GetAllFiles(path,ref fileLst);

            //获取要转化的配置表
            string[] files = fileLst.ToArray(); //Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine(files[i]);
                if (!files[i].Contains("~$") && files[i].EndsWith(".xlsx")&&files[i].Contains("_"))//xlsx
                {
                    excelList.Add(files[i]);
                    //Console.WriteLine(files[i]);
                }
            }
            
            bool result = true;
            //遍历每张配置表 进行转化操作
            for (int i = 0; i < excelList.Count; i++)
            {
                result = ReadDataForExcelTable(excelList[i]);
                if (result==false)
                {
                    Console.WriteLine($"生成失败,错误中断");
                    return;
                }
            }
            Console.WriteLine($"开始生成{fileDct.Count}个配置文件");
            fileDct[writePath + "GlobalConfig.cs"] = "namespace Game.Config\r\n{\r\n    public class GlobalConfig\r\n    {\r\n        public static int LanguageType=0;\r\n    }\r\n}";
            if (fileDct.Count>0)
            {
                //生成配置转化 cs脚本的目录
                if (Directory.Exists(writePath))
                {
                    Directory.Delete(writePath, true);
                }
                Directory.CreateDirectory(writePath);

                foreach (var item in fileDct)
                {
                    WriteFile(item.Key, item.Value);
                }
            }

           
            Console.WriteLine($"生成成功,共{fileDct.Count}配置文件");
  
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public static void WriteFile(string path, string content)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (StreamWriter sw = new StreamWriter(path, false, UTF8Encoding.UTF8))
            {
                sw.Write(content.ToString());
            }
        }

        /// <summary>
        /// 读取配置表
        /// </summary>
        /// <param name="path"></param>
        static bool ReadDataForExcelTable(string path)
        {
            //将配置表的数据 读取到 这个容器中
            DataSet dataSet;
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do {while (reader.Read()) { } } while (reader.NextResult());

                    var configuration = new ExcelDataSetConfiguration { ConfigureDataTable = tableReader => new ExcelDataTableConfiguration { UseHeaderRow = true } };
                    dataSet = reader.AsDataSet(configuration);
                }
            }
           
            List<TableEntity> tabList = new List<TableEntity>();

            #region 先缓存每张表的数据
            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                //Console.WriteLine(dataSet.Tables[i].TableName);
                var title = dataSet.Tables[i].TableName.Trim();
                if (title != "##")// if (!dataSet.Tables[i].TableName.Contains("##"))
                {
                    continue;
                }
          
                DataTable tab = dataSet.Tables[i];//表
                //转化为实体对象 记录生成逻辑必要的信息
                TableEntity tableEntity = new TableEntity();
                string filePath = Path.GetFileName(path);
                string tableName = filePath.Split('_')[1].Replace(".xlsx", "");
                //表名
                tableEntity.TableName = tableName + "Data";
                Console.WriteLine("脚本名称:" + tableName);
                
                //获取表头:注释、字段名称、类型
                for (int j = 0; j < tab.Columns.Count; j++)
                {
                    var columnName = tab.Columns[j].ColumnName;
                    var x = tab.Rows[3][columnName].ToString().Trim();
                    string type = tab.Rows[4][j].ToString().Trim();
                    if (!string.IsNullOrEmpty(x)&& type != "注释")
                    {
                        //字段注释
                        tableEntity.FieldName.Add(tab.Rows[2][columnName].ToString().Trim());
                        //字段名称
                        tableEntity.FieldList.Add(tab.Rows[3][columnName].ToString().Trim());
                        //string type = tab.Rows[4][j].ToString().Trim();

                        //类型特殊处理:json 2 string
                        if (type == "json" || type == "String")
                        {
                            type = "string";
                        }
                        if (!HasType(type))
                        {
                            Console.WriteLine($"类型不存在-------表:{path} 列:{columnName} {type}");
                            return false;
                        }
                        //记录字段类型
                        tableEntity.FieldType.Add(type);
                        tableEntity.isLog.Add(false);
                    }
                    else
                    {
                        if (type == "注释")
                        {
                            tableEntity.FieldName.Add("注释");
                            tableEntity.FieldType.Add("注释");
                            tableEntity.FieldList.Add("注释");
                            tableEntity.isLog.Add(true);
                        }
                    }
                }


                //获取实际的配置数据
                for (int x = 5; x < tab.Rows.Count; x++)
                {
                    //一行的数据 
                    List<string> row = new List<string>();
                    //一行中的每个字段
                    for (int y = 0; y < tableEntity.FieldName.Count; y++)
                    {
                        if (y == 0 && string.IsNullOrEmpty(tab.Rows[x][y].ToString()))
                        {
                            Console.WriteLine($"缺少主键:-------表:{path} 第{x + 2}行");
                            return false;
                        }
                        string str = tab.Rows[x][y].ToString();
                        str = str.Replace("\n", "").Replace("\r", "").Replace("\r\n", "").Trim();
                        if (str.StartsWith("["))
                        {
                            str = str.Replace(" ", "");
                        }
                        row.Add(str);
                    }
                    tableEntity.Rows.Add(row);
                }
                tabList.Add(tableEntity);
            }

            //CreateAllTextTable(tabList);
            #endregion


            //--------------分割线:生产实体模板--------------//

            #region 第二部分是计算生成什么代码

            string temp = @"
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    public class ClassName
    {

        static ClassName()
        {
            //初始化
            //Debug.LogError(""配置初始化:ClassName "");
            //GC.Collect();
        }

       
        //Get方法
    }

    //实体类
}
";

            //对每张表进行解析
            for (int i = 0; i < tabList.Count; i++)
            {
                //创建实体类 加到每个配置的末尾
                string templateEntity = CreateEntity(tabList[i]);
                string init = CreateInit(tabList[i]);
                string get = CreateGet(tabList[i]);
                //去掉创建实例的接口
                //string instance = CreateGetInstance(tabList[i]);
                string configTemp = temp;
                //.Replace("//创建实例",instance)
                configTemp = configTemp.Replace("ClassName", tabList[i].TableName)
                    .Replace("//初始化", init).Replace("//Get方法", get).Replace("//实体类", templateEntity);


                //Console.WriteLine(configTemp);

                #region 第三部分是将脚本文件创造出来

                //IOTools.WriteFile(writePath + tabList[i].TableName + ".cs", configTemp);
                fileDct[writePath + tabList[i].TableName + ".cs"] = configTemp;
                #endregion

                //替换到工程中
            }
            #endregion

            //生成json文件
            //for (int i = 0; i < tabList.Count; i++)
            //{
            //    CreateJsonFile(tabList[i]);
            //}
            return true;
        }

        public static Dictionary<string, string> fileDct = new Dictionary<string, string>();


        private static string CreateGet(TableEntity tableEntity)
        {
            string temp = @"
        public static Dictionary<keyType, TempEntity> all {
            get {
                return entityDic;
            }
        }
		static Dictionary<keyType, TempEntity> entityDic;
		public static TempEntity Get(keyType keyID)
		{
            try
            {
			    if (entityDic.TryGetValue(keyID,out var entity))
			    {
			    	return entity;
			    } 
            }
            catch (System.Exception x)
            {
               Debug.LogError(x.Message);
               return null;
            }
		    return null;
		}";
            temp = temp.Replace("TempEntity", tableEntity.TableName.Replace("Data", "Entity"));
            temp = temp.Replace("keyType", tableEntity.FieldType[0]);
            temp = temp.Replace("keyID", tableEntity.FieldList[0]);
            //Console.WriteLine(temp);
            return temp;
        }

        public static string CreateInit(TableEntity tableEntity)
        {
            StringBuilder init = new StringBuilder();
          
            for (int j = 0; j < tableEntity.Rows.Count; j++)
            {
                StringBuilder sb = new StringBuilder();
                if (j==0)
                {
                    sb.AppendLine($"entityDic = new Dictionary<{ tableEntity.FieldType[0]}, {tableEntity.TableName.Replace("Data", "Entity")}>({tableEntity.Rows.Count});");
                }
                string entityName = tableEntity.TableName.Replace("Data", "Entity");
                string className = "e" + j;
                string temp = "             TempEntity tempEntity = new TempEntity(//canshu);";
                //string memberInit = "           tempEntity.m_id = m_value;";

                sb.AppendLine(temp.Replace("TempEntity", entityName).Replace("tempEntity", className));

                //形参
                StringBuilder formalParameterText = new StringBuilder();
                for (int k = 0; k < tableEntity.Rows[j].Count; k++)
                {
                    if (tableEntity.isLog[k])
                    {
                        continue;
                    }
                    //字段名称都在这个里面
                    List<string> Row = tableEntity.Rows[j];
                    //如果单元格数据是空 给加个默认值
                    Row[k] = GetDefault(Row[k], tableEntity.FieldType[k]);

                    //转换成代码后的符号处理 "" f []
                    formalParameterText.Append(GetFieldGenerCode(Row[k], tableEntity.FieldType[k]));

                    if (k != tableEntity.Rows[j].Count - 1)
                    {
                        formalParameterText.Append(",");
                    }
                }
            
                sb = sb.Replace("//canshu", formalParameterText.ToString());

                string add = @"            entityDic.Add(tempEntity.ID, tempEntity);";
                add = add.Replace("tempEntity", className);
                add = add.Replace("ID", tableEntity.FieldList[0]);
                sb.AppendLine(add);
                init.Append(sb);
                //Console.WriteLine(sb);
            }

            return init.ToString();
        }

        public static string GetDefault(string input,string fieldType) {

            //默认值
            if (string.IsNullOrEmpty(input))
            {
                //Row[k] = SetDefalutValue(Row[k], tableEntity.FieldType[k]);
                switch (fieldType)
                {
                    case "int":
                    case "long":
                    case "float":
                        input = "0";
                        break;
                    case "bool":
                        input = "false";
                        break;
                    case "string":
                    case "String":
                    case "json":
                        input = "null";
                        break;
                    default:
                        input = "null";
                        break;
                }
            }
            return input;
        }

        public static string GetFieldGenerCode(string input, string fieldType,bool isJson=false) {
            StringBuilder _sb = new StringBuilder();
            if (fieldType == "string" || fieldType == "json")
            {
                if (input == "null")
                {
                    _sb.Append("null");
                }
                else
                {
                    if (isJson==false)
                    {
                        if (input.Contains(@"\n"))
                        {
                            _sb.Append($"\"{input.Replace("\"", "\"\"")}\"");
                        }
                        else
                        {
                            _sb.Append($"@\"{input.Replace("\"", "\"\"")}\"");
                        }
                    }
                    else
                    {
                        _sb.Append($"\"{input}\"");
                    }
                   
                }
            }
            else if (fieldType == "float")
            {
                if (isJson==false)
                {
                    _sb.Append(input +"f");
                }
                else
                {
                    _sb.Append(input);
                }
            }
            else if (fieldType.Contains("[]"))
            {
                if (input == "null")
                {
                    _sb.Append("null");
                }
                else
                {
                    if (fieldType.Contains("string[]"))
                    {
                        if (isJson == false)
                        {
                            _sb.Append($"new {fieldType}{{ \"{ input.Replace(",", "\",\"")}\" }}");
                        }
                        else
                        {
                            _sb.Append($"[\"{ input.Replace(",", "\",\"")}\"]");
                        }

                    }
                    else if (fieldType.Contains("float[]"))
                    {
                        if (isJson == false)
                        {
                            _sb.Append($"new {fieldType}{{{ input.Replace(",", "f,")}f}}");
                        }
                        else
                        {
                            _sb.Append($"[{ input}]");
                        }
                    }
                    else
                    {
                        if (isJson == false)
                        {
                            _sb.Append($"new {fieldType}{{{input}}}");
                        }
                        else
                        {
                            _sb.Append($"[{input}]");
                        }
                    }
                }
            }
            else
            {
                //有的配置表 这里得到的值是 真 or 假  非true 或者false 其实按else处理即可
                if (input == "真" || input.ToLower() == "true")
                {
                    _sb.Append("true");
                }
                else if (input == "假" || input.ToLower() == "false")
                {
                    _sb.Append("false");
                }
                else
                {
                    // int long ...
                    _sb.Append(input);
                }
            }
            return _sb.ToString();
        }

        public static string CreateEntity(TableEntity tableEntity)
        {
            string templateEntity = @"
    public class TemplateEntity
    {
        //TemplateMember
        public TemplateEntity(){}
        public TemplateEntity(//_canshu){
            //fuzhi
        }
    }";
            string memberInit = "           this.field = field;";
            StringBuilder entity = new StringBuilder();
            StringBuilder canshu = new StringBuilder();
            StringBuilder fuzhi = new StringBuilder();

            //遍历字段列表
            for (int i = 0; i < tableEntity.FieldList.Count; i++)
            {
                if (tableEntity.isLog[i])
                {
                    continue;
                }
                string field = tableEntity.FieldList[i];
                bool addCN = false;
                if (!tableEntity.FieldList.Contains(field+ "_en") && !tableEntity.FieldList.Contains(field + "_tw"))
                {
                    entity.AppendLine("		public " + tableEntity.FieldType[i] + " " + tableEntity.FieldList[i] + ";//" + tableEntity.FieldName[i]);
                }
                else
                {
                    addCN = true;
                    //先创建一个字段 后缀为_CN
                    entity.AppendLine("		public " + tableEntity.FieldType[i] + " " + field+"_cn" + ";//" + tableEntity.FieldName[i]);

                    //创建一个属性访问器 private
                    //entity.AppendLine("		" + tableEntity.FieldType[i] + " " + "_"+field  + ";//" + tableEntity.FieldName[i]);

                    //创建属性访问器内容
                    string _get = "get{if(GlobalConfig.LanguageType==0){return "+ field + "_cn;}" + "else if(GlobalConfig.LanguageType==1){return " + field + "_tw;}" + "else if(GlobalConfig.LanguageType==2){return " + field + "_en;}"+ "else{return null;}" + "}";
                    //string _set = "set{_"+ field+"=value;}";
                    //string _set = "set{}";

                    entity.AppendLine("		public " + tableEntity.FieldType[i] + " " +  field + "{"+_get + "}");

                }
               
                canshu.Append(tableEntity.FieldType[i] + " " + tableEntity.FieldList[i] + ((i == tableEntity.FieldList.Count - 1) ? "" : ","));
                if (addCN==false)
                {
                    fuzhi.AppendLine(memberInit.Replace("field", tableEntity.FieldList[i]));
                }
                else
                {
                    string memberInit2 = "           this.field_cn = field;";
                    fuzhi.AppendLine(memberInit2.Replace("field", tableEntity.FieldList[i]));
                }
            }

            templateEntity = templateEntity.Replace("TemplateEntity", tableEntity.TableName.Replace("Data", "Entity"));
            templateEntity = templateEntity.Replace("//TemplateMember", "//TemplateMember" + "\n" + entity.ToString());
            templateEntity = templateEntity.Replace("//_canshu", canshu.ToString());
            templateEntity = templateEntity.Replace(" //fuzhi", "\n" + fuzhi.ToString());

            //Console.Write(templateEntity);
            return templateEntity;
        }

        public static string[] types = new string[] { "int","long","float", "string","bool","json",""};
        public static bool HasType(string str) {
            str = str.ToLower();
            return types.Contains(str)||types.Contains($"{str.Replace("[]","")}");
        }

        //生成Json文件
        public static void CreateJsonFile(TableEntity tableEntity)
        {
            StringBuilder json = new StringBuilder();
            json.Append("[");
            //遍历每一行
            for (int i = 0; i < tableEntity.Rows.Count; i++)
            {
                var row = tableEntity.Rows[i];
                json.Append("{");

                //遍历字段列表
                for (int j = 0; j < tableEntity.FieldList.Count; j++)
                {
                    if (tableEntity.isLog[j])
                    {
                        continue;
                    }
                    string key = $"\"{tableEntity.FieldList[j]}\"" ;//字段名称

                    row[j] = GetDefault(row[j], tableEntity.FieldType[j]);
                    //转换成代码后的符号处理 "" f []
                    var v= GetFieldGenerCode(row[j], tableEntity.FieldType[j],true);
                    json.Append($"{key}").Append(":").Append(v);
                    if (j != tableEntity.FieldList.Count - 1)
                    {
                        json.Append(",");
                    }
                }
                json.Append("}");
                if (i!= tableEntity.Rows.Count-1)
                {
                    json.Append(",");
                }
            }
            json.Append("]");

            //writePath
            var targetDirectory = jsonPath;// writePath + "/../" + "JsonFile";
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            var targetFile = targetDirectory +"/"+ tableEntity.TableName + ".txt";
            using (StreamWriter streamWriter = new StreamWriter(targetFile,false, Encoding.UTF8))
            {
                streamWriter.Write(json.ToString());
                Console.WriteLine(targetFile);
            }

        }


        public static void CreateAllTextTable(List<TableEntity> tabList)
        {
            //1.扫描所有的表单
            //2.判定每个字段,是否还存在结尾是_tw和_en的.
            //3.如果存在就记录索引,然后遍历每张表的每一行
            //4.将数据压入到简体 繁体 英文的字典中

            //不同字段 对应的索引
            List<int> cn_index = new List<int>();
            List<int> tw_index = new List<int>();
            List<int> en_index = new List<int>();

            //所有待生成的字符串
            List<int> type=new List<int>();
            List<string> cn = new List<string>();
            List<string> tw = new List<string>();
            List<string> en = new List<string>();

            foreach (var item in tabList)
            {
                Dictionary<string, int> fIndex = new Dictionary<string, int>();
                //先确定所有字段名称 对应的 索引
                for (int i = 0; i < item.FieldList.Count; i++)
                {
                    fIndex[item.FieldList[i]] = i;
                }


                //确定好索引
                for (int i = 0; i < item.FieldList.Count; i++)
                {
                    var item2 = item.FieldList[i];
                    if (item.FieldList.Contains(item2)&& item.FieldList.Contains(item2+"_tw") && item.FieldList.Contains(item2 + "_en"))
                    {
                        //类型
                        if (item.FieldType[i].Contains("["))
                        {
                            type.Add(1);
                        }
                        else
                        {
                            type.Add(0);
                        }

                        cn_index.Add(fIndex[item2]);
                        tw_index.Add(fIndex[item2 + "_tw"]);
                        en_index.Add(fIndex[item2 + "_en"]);
                       
                    }
                }

                //遍历所有索引 缓存对应的文本
                var rowsContent = item.Rows;
                //遍历每一行
                foreach (var r in rowsContent)
                {
                    //索引集合 拿到对应的数据
                    for (int i = 0; i < cn_index.Count; i++)
                    {
                        //添加这一行中的简体 繁体 英文
                        cn.Add(r[cn_index[i]]);
                        tw.Add(r[tw_index[i]]);
                        en.Add(r[en_index[i]]);
                    }
                }
            }


            Dictionary<string, string> cn_tw = new Dictionary<string, string>();
            Dictionary<string, string> cn_en = new Dictionary<string, string>();
            //所有表都走完了 开始去重
            for (int i = 0; i < cn.Count; i++)
            {
                if (type[i]==0)
                {
                    if (cn_tw.ContainsKey(cn[i])==false)
                    {
                        cn_tw.Add(cn[i], tw[i]);
                        cn_en.Add(cn[i], en[i]);
                    }
                }
                else
                {
                    //数组
                    var array = cn[i];
                    if (string.IsNullOrEmpty(array) == false && array.ToLower()!="null")
                    {
                        var strs = array.Split(",");

                        var tw_str = (tw[i] == null|| tw[i].ToLower() != "null") ? null : tw[i].Split(",");
                        var en_str = (en[i] == null || en[i].ToLower() != "null") ? null : en[i].Split(",");

                        for (int j = 0; j < strs.Length; j++)
                        {
                            var cn1 = strs[j];
                            //var t = tw_str[j];
                            //var e = en[j];
                            if (cn_tw.ContainsKey(cn1) == false)
                            {
                                cn_tw.Add(cn1, tw_str.Length > j ? tw_str[j]:"");
                                cn_en.Add(cn1,en_str.Length> j ? en_str[j]:"");
                            }
                        }
                    }
                }
            }


            TableEntity tableEntity = new TableEntity();
            tabList.Add(tableEntity);
            tableEntity.TableName = "AllTextTable";

            tableEntity.FieldName.Add("简体");
            tableEntity.FieldName.Add("繁体");
            tableEntity.FieldName.Add("英文");

            tableEntity.FieldList.Add("t1");
            tableEntity.FieldList.Add("t2");
            tableEntity.FieldList.Add("t3");

            tableEntity.FieldType.Add("string");
            tableEntity.FieldType.Add("string");
            tableEntity.FieldType.Add("string");

            tableEntity.isLog.Add(false);
            tableEntity.isLog.Add(false);
            tableEntity.isLog.Add(false);

            List<string> text1 = new List<string>();
            List<string> text2 = new List<string>();
            List<string> text3 = new List<string>();

            foreach (var item in cn_tw)
            {
                text1.Add(item.Key);
                text2.Add(item.Value);
            }

            foreach (var item in cn_en)
            {
                text3.Add(item.Value);
            }
      
            for (int i = 0; i < text1.Count; i++)
            {
                var l = new List<string>();
                l.Add(text1[i]);
                l.Add(text2[i]);
                l.Add(text3[i]);
                tableEntity.Rows.Add(l);
            }

        }

    }
}

public class TableEntity
{
    public string TableName;//表名
    public List<string> FieldName = new List<string>();//字段注释 索引0
    public List<string> FieldType = new List<string>();//字段数据类型 索引1
    public List<string> FieldList = new List<string>();//字段名称 索引2
    public List<bool> isLog = new List<bool>();
    public List<List<string>> Rows = new List<List<string>>();//每一行的集合
}
