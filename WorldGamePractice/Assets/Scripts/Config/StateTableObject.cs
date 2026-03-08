using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Config;
using Unity.VisualScripting.FullSerializer;
using JetBrains.Annotations;



[System.Serializable]
public enum E_StateEvent
{
    空闲,
    攻击,
    跳跃,
    移动,
    急停,
    翻滚,
    技能,

}
[System.Serializable]
public enum E_StateType
{
    主动状态,
    被动状态,
    自身被动状态
}

[System.Serializable]
public enum E_CharacterTriggerPiont
{
    前方,
    右手,
    左手,
    头顶,
    脚下,
    背后,
    /// <summary>
    /// 相机挂点
    /// </summary>
    焦点,
}

[System.Serializable]
public enum E_EffectType
{
    //被动类型
    /// <summary>
    /// 斩击
    /// </summary>
    斩击,



    //自身被动类型
    /// <summary>
    /// 弹反
    /// </summary>
    弹反=1000,
}

[System.Serializable]
public enum E_CharacterBoxType
{
    默认,
    减半,
    穿过玩家层,
}


[CreateAssetMenu(menuName = "配置/状态配置/创建空角色状态配置")]
public class StateTableObject: ScriptableObject
{
    [SerializeField]
    public List<StateEntity> states = new List<StateEntity>();
}

[System.Serializable]
public class StateEntity
{
    [Header("==============================================")]
    [Header("==============================================")]
    [Header("状态名称")]
    public string statename;
    [Header("状态对应动画片段名称")]
    public string animClipName;
    [Header("状态优先级")]
    public int statusPriority;
    [Header("状态类型")]
    public E_StateType type;
    [Header("是否需要等待")]
    public bool isWait;
    [Header("状态变换控制力（0-1）")] 
    public float stateChangeTime;
     public Vector2 stateChangeForce;//到另外一个状态的融合度
    [Space(20)]
    [Header("状态位移控制力（-1-0）")]
    public AddFloatTemp stateMoveForce;
    public AddFloatTemp stateMoveForcer;
    [Header("状态位移抵抗力（0-1）")]
    public AddFloatTemp statePassiveMoveForce;
    public AddFloatTemp statePassiveMoveForcer;
    //[Header("角色控制器相关的信息")]
    //public E_CharacterBoxType characterBox = 0;
    ////public AddVector3Temp characterBox=new AddVector3Temp();
    //[Header("忽略的层级")]
    //public string[] ignoreLayer;
    [Header("------------------------")]
    [Header("施加额外硬直（第二个参数0为动画时间）")]
    public float extraStiffness;
     public float extraStiffnesstime;
    [Header("被动状态失衡临界值")]
     public float imBanlance;
    [Header("被动状态受击方向（0-1）")]
    public Vector3 directionChangeForce;
    [Header("------------------------")]
    [Header("施加速度")]
    [SerializeField]
    public AddListVector3Temp addSpeedInStates = new AddListVector3Temp();
    [Header("施加效果")]
    public bool isAddCharacterAttributes;
    [SerializeField]
    public List<AddEffectInState> addEffectInStates = new List<AddEffectInState>();
    [Header("施加音效")]
    [SerializeField]
    public AddSoundEffect soundEffect = new AddSoundEffect();
    [Header("施加相机效果")]
    [SerializeField]
    public AddCameraEffect cameraEffect = new AddCameraEffect();
    [Header("------------------------")]
    [Header ("状态事件")]
    [SerializeField]
    public List<StateEventConfig> stateEventConfigs = new List<StateEventConfig>();
}

[System.Serializable]
public class AddVector3Temp
{
    [Header("增值大小和方向")]
    public Vector3 value;
    [Header("施加时间（0为状态时间）")]
    public float time;
    [Header("变化类型")]
    public BuffCurveType buffCurveType;
    [Header("叠加类型")]
    public BuffStackType buffStackType;
    [Header("增值算法")]
    public BuffOperationType buffOperationType;

    public AddVector3Temp Clone()
    {
        return new AddVector3Temp
        {
            value = this.value,
            time = this.time,
            buffCurveType = this.buffCurveType,
            buffStackType = this.buffStackType,
            buffOperationType = this.buffOperationType
        };
    }
}
[System.Serializable]
public class AddListVector3Temp
{
    [Header("增值大小和方向")]
    public List<DIYBuff<Vector3>> buffs;
    [Header("施加时间（0为状态时间）")]
    public float time;
    [Header("叠加类型")]
    public BuffStackType buffStackType;
    [Header("增值算法")]
    public BuffOperationType buffOperationType;


}

[System.Serializable]
public class AddFloatTemp
{
    [Header("增值大小")]
    public float value;
    [Header("施加时间(0就是动画时间)")]
    public float time;
    [Header("叠加类型")]
    public BuffStackType buffStackType;
    [Header("变化类型")]
    public BuffCurveType buffCurveType;
    [Header("增值算法")]
    public BuffOperationType buffOperationType;
}
[System.Serializable]
public class AddEffectInState
{
    [Header("位置")]
    public E_CharacterTriggerPiont piont;
    [Header("配置的资源路径（空的话就查找自身）")]
    public string path="";
    [Header("开始时间")]
    public float starttime;
    [Header("结束时间")]
    public float endtime;
    [Space(20)]
    [Header("是否开启触发器")]
    public bool isOpenCheck;
    [Header("触发效果类型")]
    public E_EffectType effectType;
    [Header("检测触发开始时间")]
    public float triggerstarttime;
    [Header("检测触发次数")]
    public int triggernumber;
    [Header("检测触发结束时间")]
    public float triggerendtime;
    [Header("作用在触发器上的每次变化施力")]
    public AddVector3Temp stateChangeOpponentForce = new AddVector3Temp();
    //[Header("作用在触发器上的每次变化施力（对其自身）")]
    //public AddVector3Temp selfstateChangeOpponentForce = new AddVector3Temp();
    [Header("作用在触发器上的每次速度施力")]
    public AddVector3Temp stateMoveOpponentForce = new AddVector3Temp();
    [Space(20)]
    [Header("作用在触发器上的每属性变化")]
    public CharacterAttributes characterAttributes=new CharacterAttributes();
}
[System.Serializable]
public class AddCameraEffect
{
    [Header("触发开始时间")]
    public float starttime;
    [Header("变化类型")]
    public int changeType;
    [Header("与主摄像机最大触发距离")]
    public float distance;
}
[System.Serializable]
public class AddSoundEffect
{
    [Header("触发开始时间")]
    public float starttime;
    [Header("配置的资源路径")]
    public string path;
    [Header("与主摄像机最大触发距离")]
    public float distance;
}

[System.Serializable]
public class StateEventConfig
{
    public E_StateEvent e_StateEvent;
    public string toStateEntityname = "";
}

[System.Serializable]
public class CharacterAttributes
{
    [Header("血量")]
    public float Health;
}
