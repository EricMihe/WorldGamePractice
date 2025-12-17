using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Windows;

public interface BaseFSM
{
    Character Character { get; set; }
    State CurrentState {  get; set; }
    

    void Open();
    void Running();
    void Close();
    void ChangeState();
    void GetCurrentState();
    void StateEnter();
    void StateExcute();
    void StateExit();
}

public class State
{

}

public class BaseCharacter : MonoBehaviour
{
    BaseFSM _fsm;
    Animator _animator;
    CharacterController _characterController;

    /// <summary>
    /// 角色身上的特效（碰撞器）挂点位置
    /// </summary>
    List<Transform> _effectAttachmentPoint;
    Transform _cameraPoint;
    public bool isOnCamera;


    /// <summary>
    /// 初始化角色，在Awake进行调用
    /// </summary>
    /// <param name="fsm"></param>
    public void Init(BaseFSM fsm)
    {
        
    }

    /// <summary>
    /// 执行死亡逻辑
    /// </summary>
    public void OnDead()
    {

    }

    Dictionary<E_EventName ,bool> stateTransitionListenerFlag=new Dictionary<E_EventName ,bool>();
    
    /// <summary>
    /// 监听输入信息，传入被输入系统触发的事件的名字，并且获取触发对应的输入
    /// </summary>
    /// <param name="e_EventName"></param>
    public void MonitorInputInformation(E_EventName e_EventName,InputInfo inputInfo)
    {
        //根据事件将标志设置为true
        //将得到的输入信息设置记录，用于更新角色的状态和移动
    }

    Vector3 _currentStateControl;
    /// <summary>
    /// 切换角色播放的动画，添加状态配置的平衡控制力，或者消掉部分失衡控制力
    /// </summary>
    public void Change()
    {

    }

    Vector3 _currentMoveControl;
    public void Move()
    {

    }
    public void AddBalanceMoveForce()
    {

    }
    public void AddBalanceChangeForce()
    {

    }
    public void AddMoveForce()
    {

    }
    public void AddChangeForce()
    {

    }

    bool _isOpenGravity;
    bool _isOnGround;
    /// <summary>
    /// 重力施加，只有在开启时才启用
    /// </summary>
    public void UsingGravity()
    {
        if (_isOpenGravity&&!_isOnGround)
        {

        }
    }

    /// <summary>
    /// 作用在碰撞器上的每次对对方的施力
    /// </summary>
    public void  ProactiveAction()
    {

    }

    /// <summary>
    /// 每次被攻击，导致改变角色当前的属性，从而改变角色的状态
    /// </summary>
    public void PassiveAction(Character enemy)
    {

    }
    /// <summary>
    /// 自己施加给自己的被动行为
    /// </summary>
    public void SelfPassiveAction()
    {

    }

    /// <summary>
    /// 调整角色的形状
    /// </summary>
    public void ChangeCharacterBox()
    {

    }
    /// <summary>
    /// 是否开启碰撞器，根据碰撞器触发时间开关碰撞器
    /// 然后在碰撞检测函数中去延迟执行施力（通过碰撞时间和次数进行计算）
    /// </summary>
    public void EffectOpen()
    {

    }
    /// <summary>
    /// 添加摄像机的控制，什么类型的控制，是否需要当前角色挂载有主摄像机，需要离主摄像机多近才能触发
    /// </summary>
    public void ControlCamera()
    {

    }

    /// <summary>
    /// 添加音效，离主摄像机多近才能听到
    /// </summary>
    /// <param name="mainCameraDistance"></param>
    public void AddSoundEffects(Transform mainCameraDistance)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        //可以在开启时就把碰撞器上的检测脚本存储起来
    }

    // Update is called once per frame
    void Update()
    {
        //当前状态是否开启碰撞器，然后去更新EffectTime

        //更新状态机
    }
}
