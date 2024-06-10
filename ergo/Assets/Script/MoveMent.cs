using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
//Version:1.0  Editer： 吴顺吉  Using for:VR跌倒干预试验
//Version:1.1  Editer:  朱一帆  Using for:VR环境下空间认知实验
//Version:2.0  Editer:  朱一帆  Using for:Msc-exp1
//Version:2.1  Editer:  朱一帆  Using for:Undergraduated Course Ergonomic [*GE00703]


public class MoveMent : MonoBehaviour
{
    [SerializeField] float recordTimmer;
    [SerializeField] Transform target_head;
    [SerializeField] Transform target_body;


    //跨物体读取
    [SerializeField] GameObject NPC_1;
    //GameObject.Find(“XXX”).GetComponent<脚本>().变量;


    float timmer;
    string targetDataAll;
    private void Start()
    {
        timmer = recordTimmer;
    }

    bool isEnd = false;
    int dataallending = 0;
    void Presstoend()
    {
        if (Input.GetKey(KeyCode.F2) && isEnd==false )
        {
            dataallending += 1;
            print("Getting to ending data\n");
            if (dataallending == 5) { 
                print("END!\n");
                isEnd = true;
            }
            return;
        }
        if (isEnd == true)
        {
            string filePath = Application.streamingAssetsPath;
            Debug.Log("filePath:" + filePath);
            string fileName = System.DateTime.Now.ToString("yyyy-MM-dd").Replace("-", "").Trim() + System.DateTime.Now.ToString("hh:mm:ss").Replace(":", "").Trim() + ".txt";
            CreateTextToFile(filePath, fileName, targetDataAll);
            isEnd = false;
        }
    }

    //启动标志
    int datastartstreaming = 0;
    private void Update()
    {
        //长按键盘F1，触发计时
        if (Input.GetKey(KeyCode.F1)&& datastartstreaming<6) { 
            datastartstreaming += 1;
            print("Getting to streaming data\n");
            if(datastartstreaming==5) print("start!\n");
        }
        
        if (datastartstreaming>=5)//remoter2.GetChanged(pose.inputSource)
        {
            timmer -= Time.deltaTime;
            if (timmer < 0)
            {
                timmer = recordTimmer;
                string data = GetTargetStr(target_body.position, target_body.eulerAngles, target_head.eulerAngles) + "\n";//target.position.ToString() + target.eulerAngles + "\n";
                targetDataAll += data;
            }
        }
        Presstoend();
    }


    /// <summary>
    /// 创建文件
    /// <param name="filePath"></param>
    /// <param name="data"></param>
    public void CreateTextToFile(string filePath, string fileName, string data)
    {
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        if(!Directory.Exists(filePath + "\\All"))Directory.CreateDirectory(filePath + "\\All");

        string fileAllPath = Path.Combine(filePath + "\\All", fileName);

        StreamWriter swAll = new StreamWriter(fileAllPath, false, Encoding.UTF8);
        swAll.WriteLine(targetDataAll);
        swAll.Close();
        swAll.Dispose();
    }

    string GetTargetStr(Vector3 pos, Vector3 angle_body, Vector3 angle_head)
    {
        string value = "";
        value =  pos.x + "," + pos.y + "," + pos.z + "," + angle_body.y + "," + angle_head.x ;
        return value;
            
    }


}
