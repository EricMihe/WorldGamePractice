using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using ReadOnlyAttribute = Unity.Collections.ReadOnlyAttribute;


public class BaseCharacter : MonoBehaviour
{
    [Header("角色唯一ID")]
    public string id;
    [Header ("引用的状态配置")]
    public StateTableObject stateTableObject;
    [Header("是否相机控制状态")]
    public bool onCameraControl=false;

    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public CharacterController controller;
    [HideInInspector]
    /// <summary>
    /// 角色身上的特效挂点位置
    /// </summary>
    public Dictionary<E_CharacterTriggerPiont ,Transform > _triggerPiont;
    Dictionary<E_CharacterTriggerPiont, Collider> _triggerPiontCollider;
    
    BaseFSM _fsm;
    public bool isOpenMove = true;
    public float characterSpeed = 10f;


    //用于处理移动
    float currentmoveSpeedScale;
    float currentmoveSpeedScaler;
    float currentpassiveMoveSpeedScale;
    float currentpassiveMoveSpeedScaler;
    Vector3 currentactiveStackingMove;
    Vector3 currentpassiveStackingMove;
    Vector3 currentselfpassiveStackingMove;

    // 用于处理受击
    [HideInInspector]
    public float currenResilience;
    [HideInInspector]
    public Vector3 currentpassiveForce = Vector3.zero;
    [HideInInspector]
    public Vector3 currentselfpassiveForce = Vector3.zero;


    bool _isOpenGravity=true;
    bool _isOnGround;
    bool _isUisngGravity=false;
    [HideInInspector] public int moveSpeedScale;
    [HideInInspector] public int moveSpeedScaler;
    [HideInInspector] public int passiveMoveSpeedScale;
    [HideInInspector] public int passiveMoveSpeedScaler;
    [HideInInspector]public int activeStackingMove;
    [HideInInspector]public int passiveStackingMove;
    [HideInInspector]public int selfpassiveStackingMove;
    [HideInInspector]public int currentResilience;
    [HideInInspector]public int passiveForce;
    [HideInInspector]public int selfpassiveForce;


    int currentHeatlh = 100;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    void OnDisable()
    {

        if (activeCoroutines!=null && activeCoroutines.Count >0)
        {
            foreach (var c in activeCoroutines.Values)
            {
                if (c != null) StopCoroutine(c);
            }
            activeCoroutines.Clear();
        }
        
   
    }
    // Update is called once per frame
    void Update()
    {
        UpdateCurrentAttribute();
        if (onCameraControl)
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                _fsm.ActiveStateTrigger(E_StateEvent.攻击);
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                _fsm.ActiveStateTrigger(E_StateEvent.跳跃);
            }
            if (playerMove.magnitude < 0.1f)
            {
                _fsm.ActiveStateTrigger(E_StateEvent.急停);
            }
        }


        _fsm.Run();
        if(isOpenMove)Move();
        UnityEngine.Debug.Log($"受击力:{currentpassiveForce}");
    }

    private Dictionary<Collider, Coroutine> activeCoroutines = new Dictionary<Collider, Coroutine>();
    //BaseCharacter _currentActtckTagret = null;
    //bool isCheck = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.IsChildOf(transform)&&other.transform!=transform)
        {

            //添加触发类型，是攻击，弹反，还是治疗等
            if (true)
            {
                // 如果已存在，先停止（防止重复进入）
                if (activeCoroutines.ContainsKey(other) && activeCoroutines[other] != null) StopCoroutine(activeCoroutines[other]);
                activeCoroutines[other] = StartCoroutine(ApplyHitOverTime(other));
            }
             
   
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (!other.transform.IsChildOf(transform) && other.transform != transform)
        {
            if (activeCoroutines.TryGetValue(other, out Coroutine c))
            {
                //if (stopWhenExit)
                //{
                //    StopCoroutine(c);
                //}

                UnityEngine.Debug.Log($"{other.name}22222222");
                if (c != null) StopCoroutine(c);
                activeCoroutines.Remove(other);
            }
        }         

    }

    /// <summary>
    /// 初始化角色
    /// </summary>
    /// <param name="fsm"></param>
    public void Init()
    {
        animator= GetComponentInChildren<Animator>();
        if (animator == null) throw new ArgumentException($"{animator} not found");
        controller = GetComponentInChildren<CharacterController>();
        if (controller == null) throw new ArgumentException($"{controller} not found");

        _triggerPiont = new Dictionary<E_CharacterTriggerPiont, Transform>();
        _triggerPiontCollider = new Dictionary<E_CharacterTriggerPiont, Collider>();
        Array values = Enum.GetValues(typeof(E_CharacterTriggerPiont));
        foreach (E_CharacterTriggerPiont value in values)
        {
            
            Transform point = transform.FindChildByName(value.ToString());
            //Debug .Log (value.ToString());
            if (point != null)
            {
                _triggerPiont.Add(value, point);

                var collider = point.GetComponentInChildren<Collider>();
                if (collider!=null)
                {
                    _triggerPiontCollider[value] = collider;
                    collider.enabled = false;
                }

                //point.gameObject .SetActive(false);
            }
        }

        if(onCameraControl)
        {
            var c = Camera.main.transform.GetComponentInChildren<CameraController>();
            if(c != null&& _triggerPiont.ContainsKey (E_CharacterTriggerPiont.焦点)&& _triggerPiont[E_CharacterTriggerPiont.焦点]!=null)
            {
                c.targetPoint = _triggerPiont[E_CharacterTriggerPiont.焦点];
            }

            EventCenter.Instance.AddEventListener<float>(WorldEventType.E_Input_Horizontal, DO_Move_x);
            EventCenter.Instance.AddEventListener<float>(WorldEventType.E_Input_Vertical, DO_Move_y);
            EventCenter.Instance.AddEventListener(WorldEventType.E_Input_Attack1, DO_Attack);
        }



        //创建对应的动态属性
        moveSpeedScale = BuffMgr.Instance.AddValue(1f);
        moveSpeedScaler = BuffMgr.Instance.AddValue(1f);

        passiveMoveSpeedScale = BuffMgr.Instance.AddValue(0f);
        passiveMoveSpeedScaler = BuffMgr.Instance.AddValue(0f);

        activeStackingMove = BuffMgr.Instance.AddValue<Vector3>(Vector3.zero);
        passiveStackingMove = BuffMgr.Instance.AddValue<Vector3>(Vector3.zero);
        selfpassiveStackingMove = BuffMgr.Instance.AddValue<Vector3>(new Vector3(0, -2f, 0));
        currentResilience = BuffMgr.Instance.AddValue<float>(0f);
        passiveForce = BuffMgr.Instance.AddValue<Vector3>(Vector3.zero);
        selfpassiveForce = BuffMgr.Instance.AddValue<Vector3>(Vector3.zero);

        if (_fsm == null)
        {
            _fsm = new FSM();

            _fsm.Init(this);
        }

    }

    void UpdateCurrentAttribute()
    {
        currentmoveSpeedScale = BuffMgr.Instance.GetValue<float>(moveSpeedScale);
        currentmoveSpeedScaler = BuffMgr.Instance.GetValue<float>(moveSpeedScaler);

        currentpassiveMoveSpeedScale = BuffMgr.Instance.GetValue<float>(passiveMoveSpeedScale);
        currentpassiveMoveSpeedScaler = BuffMgr.Instance.GetValue<float>(passiveMoveSpeedScaler);

        currentactiveStackingMove = BuffMgr.Instance.GetValue<Vector3>(activeStackingMove) ;
        currentpassiveStackingMove = BuffMgr.Instance.GetValue<Vector3>(passiveStackingMove);

        currentselfpassiveStackingMove = BuffMgr.Instance.GetValue<Vector3>(selfpassiveStackingMove);
        currenResilience = BuffMgr.Instance.GetValue<float>(currentResilience);
        currentpassiveForce = BuffMgr.Instance.GetValue<Vector3>(passiveForce) ;
        currentselfpassiveForce = BuffMgr.Instance.GetValue<Vector3>(selfpassiveForce);

        //if (onCameraControl)
        //{
        //    playerMove.x = UnityEngine.Input.GetAxis("Horizontal");
        //    playerMove.y = UnityEngine.Input.GetAxis("Vertical");
        //}
    }

    Vector2 playerMove = new Vector2(0, 0);
    Vector3 targetDirection;
    Vector3 do_move;
    float turnSmoothVelocity;
    Transform cam;
    /// <summary>
    /// 控制角色移动
    /// </summary>
    public virtual void Move()
    {
        if (onCameraControl)
        {

            if (playerMove.normalized.magnitude >= 0.1f)
            {
                // 获取主相机的水平朝向（忽略Y轴）
                float targetAngle = Mathf.Atan2(playerMove.x, playerMove.y) * Mathf.Rad2Deg;
                cam = Camera.main.transform;
                //Mathf.Atan2 正切函数 求弧度 * Mathf.Rad2Deg(弧度转度数) >> 度数

                //第一:先求出输入的角度
                //第二:加上当前相机的Y轴旋转的量
                //第三:得到目标朝向的角度
                targetAngle = Mathf.Atan2(playerMove.x, playerMove.y) * Mathf.Rad2Deg +
                                  cam.eulerAngles.y;

                //做一个插值运动
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                    1.05f / characterSpeed + 1f - Mathf.Clamp01(currentmoveSpeedScaler));

                //角色先旋转到目标角度去
                // rotate to face input direction relative to camera position
               transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                //计算目标方向 通过这个角度
                targetDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;

                targetDirection = targetDirection.normalized;

            }

        }
        if (currentpassiveMoveSpeedScaler > 0.05f)
        {
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, Mathf.Clamp01(currentpassiveMoveSpeedScaler) , ref turnSmoothVelocity,
                    0.15f);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        _isOnGround = controller.isGrounded;
        if (_isOnGround && currentselfpassiveStackingMove.y < 0)
        {
            BuffMgr.Instance.RemoveBuff<Vector3>(selfpassiveStackingMove, "Gravity");
            BuffMgr.Instance.SetBaseValue<Vector3>(selfpassiveStackingMove, new Vector3(0, -2f, 0));
            _isUisngGravity = false;
        }
        if (_isOpenGravity && !_isOnGround)
        {
            if (!_isUisngGravity)
            {
                _isUisngGravity = true;
                BuffMgr.Instance.SetBaseValue<Vector3>(selfpassiveStackingMove, new Vector3(0, -9.8f, 0));
                //BuffMgr.Instance.AddBuff<Vector3>(selfpassiveStackingMoveName, new Vector3(0, -99f, 0), 999f, BuffCurveType.Instant, BuffStackType.Basic, BuffOperationType.Additive, "Gravity");
            }
        }

        currentpassiveStackingMove*= 1f- Mathf.Clamp01(currentpassiveMoveSpeedScale);
        targetDirection *= Mathf.Clamp01(currentmoveSpeedScale) * characterSpeed;
        do_move += currentpassiveStackingMove;
        do_move += currentselfpassiveStackingMove;
        do_move += currentactiveStackingMove;
        do_move = isUsingOtherMoveForward? targetMoveForward.TransformDirection(do_move): transform.TransformDirection(do_move);


        do_move += targetDirection;
        controller.Move(do_move * Time.deltaTime);
        playerMove = Vector2.zero;
        do_move = Vector3.zero;
        targetDirection= Vector3.zero;
    }


    //用于处理攻击
    [HideInInspector]
    public AddVector3Temp AttackChangeOpponentForce;
    [HideInInspector]
    public AddVector3Temp AttackMoveOpponentForce;
    //用于处理类型
    [HideInInspector]
    public E_EffectType stateOpponentType;
    //攻击次数
    [HideInInspector]
    public int triggerNumber;
    [HideInInspector]
    public float triggerTime;

    Dictionary<GameObject, int> removeEffect = new Dictionary<GameObject, int>();
    Dictionary<Collider,int> removeTimeWithCollider = new Dictionary<Collider, int>();
    public void OpenTriggerPiontCollider(AddVector3Temp stateChangeOpponentForce, AddVector3Temp stateMoveOpponentForce, E_EffectType stateOpponentType,E_CharacterTriggerPiont e_StateTriggerPiont,float time, int number = 1)
    {
        this.AttackChangeOpponentForce=stateChangeOpponentForce;
        this.stateOpponentType=stateOpponentType;
        this .AttackMoveOpponentForce= stateMoveOpponentForce;
        if (number <= 0) number = 1;
        triggerNumber= number;
        triggerTime= time;
        if (_triggerPiontCollider[e_StateTriggerPiont] != null)
        {
            _triggerPiontCollider[e_StateTriggerPiont].enabled = true;
            if(removeTimeWithCollider.ContainsKey(_triggerPiontCollider[e_StateTriggerPiont])) TimerMgr.Instance.RemoveTimer(removeTimeWithCollider[_triggerPiontCollider[e_StateTriggerPiont]]);
            removeTimeWithCollider[_triggerPiontCollider[e_StateTriggerPiont]]=TimerMgr.Instance.CreateTimerWithParam<Collider>(time, CloseEffectCollider, _triggerPiontCollider[e_StateTriggerPiont]);
        }

            
        //else TimerMgr.Instance.CreateTimer(time, RemoveEffect, true);
    }

    public void OpenTriggerWithAddEffectCollider(AddVector3Temp stateChangeOpponentForce, AddVector3Temp stateMoveOpponentForce, E_EffectType stateOpponentType, string path,E_CharacterTriggerPiont e_StateTriggerPiont, float time, int number = 1)
    {
        this.AttackChangeOpponentForce = stateChangeOpponentForce;
        this.stateOpponentType = stateOpponentType;
        this.AttackMoveOpponentForce = stateMoveOpponentForce;
        if (number <= 0) number = 1;
        triggerNumber = number;
        triggerTime = time;
        string[] strings = path.Split('/');
        string newpath = "";
        for (int i = 0; i < strings.Length; i++)
        {
            newpath += "_" + strings[i];
        }
        var triggerTransform = _triggerPiont[e_StateTriggerPiont].Find(newpath);
        if (triggerTransform != null)
        {
            var triggercollider = triggerTransform.GetComponentInChildren<Collider>();
            if (triggercollider != null) triggercollider.enabled = true;
            if (removeTimeWithCollider.ContainsKey(triggercollider)) TimerMgr.Instance.RemoveTimer(removeTimeWithCollider[triggercollider]);
            removeTimeWithCollider[triggercollider] = TimerMgr.Instance.CreateTimerWithParam<Collider>(time, CloseEffectCollider, triggercollider);
        }
      
    }

    void CloseEffectCollider(Collider collider)
    {
        if (collider != null) collider.enabled = false;
        triggerNumber = 0;
        triggerTime = 0;
        AttackChangeOpponentForce = null;
        AttackMoveOpponentForce = null;
        stateOpponentType = default;
       if(removeTimeWithCollider.ContainsKey(collider)) TimerMgr.Instance.RemoveTimer(removeTimeWithCollider[collider]);

    }
    public void CloseAllEffectCollider()
    {
        if (_triggerPiontCollider.Count > 0)
        {
            foreach (var item in _triggerPiontCollider)
            {
                CloseEffectCollider(item.Value);
            }

        }
    }

    public void CloseAllWithAddEffectCollider()
    {
        if (removeEffect.Count > 0)
        {

            foreach (var item in removeEffect)
            {
                if (item.Key != null)
                {
                    var c = item.Key.GetComponentInChildren<Collider>();
                    if (removeTimeWithCollider.ContainsKey(c)) CloseEffectCollider(c);


                    CloseEffectCollider(c);
                }
            }
        }

        
    }

    public void AddEffcetOnTriggerPiont(float time, string path, E_CharacterTriggerPiont e_)
    {

        var obj = ResMgr.Instance.Load<GameObject>(path);
        if (obj != null)
        {
            var instance = UnityEngine.Object.Instantiate(obj, _triggerPiont[e_]);
            string[] strings = path.Split('/');
            string newpath = "";
            for (int i = 0; i < strings.Length; i++)
            {
                newpath += "_" + strings[i];
            }
            instance.name = newpath;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            var triggercollider = instance.GetComponentInChildren<Collider>();
            if (triggercollider != null) triggercollider.enabled = false;     
            removeEffect[instance] = TimerMgr.Instance.CreateTimerWithParam<GameObject>(time, ReMoveWithAddEffect, instance);

        }

    }

    public void UpdateTriggerPiontCollider(E_CharacterTriggerPiont e_CharacterTriggerPiont,Collider collider)
    {
        if(_triggerPiontCollider.ContainsKey(e_CharacterTriggerPiont))
        {

            CloseEffectCollider(_triggerPiontCollider[e_CharacterTriggerPiont]);
            removeTimeWithCollider.Remove(_triggerPiontCollider[e_CharacterTriggerPiont]);
        }
        _triggerPiontCollider[e_CharacterTriggerPiont]= collider;

    }

    public void ReMoveWithAddEffect(GameObject gameObject)
    {

        if (gameObject != null)
        {

            GameObject.Destroy(gameObject);
            TimerMgr.Instance.RemoveTimer(removeEffect[gameObject]);
            removeEffect.Remove (gameObject);
        }
        else UnityEngine.Debug.Log("不包含此对象");

    }
    public void ReMoveAllWithAddEffect()
    {
        if (removeEffect.Count > 0)
        {
            foreach (var item in removeEffect)
            {
                if(item.Key != null)
                {
                    CloseEffectCollider(item.Key.gameObject.GetComponentInChildren<Collider>());
                    ReMoveWithAddEffect(item.Key);
                }
            }
            removeEffect.Clear();
        }
       
    }


    /// <summary>
    /// 持续受到施加伤害
    /// </summary>
    IEnumerator ApplyHitOverTime(Collider target)
    {
        var character = target.GetComponentInParent<BaseCharacter>();
        if (character!=null)
        {

            int tickCount = 1;
            AddVector3Temp currentchangeForce = null;
            AddVector3Temp currentmoveForce = null;
            int number = character.triggerNumber;
            float time = character.triggerTime;
            if (character.AttackChangeOpponentForce != null)
            {
                currentchangeForce = character.AttackChangeOpponentForce.Clone();
               currentchangeForce.value = -character.transform.TransformDirection(currentchangeForce.value);
            }
            else UnityEngine.Debug.Log("没有施加改变速度");
            if (character.AttackMoveOpponentForce != null)
            {
                currentmoveForce = character.AttackMoveOpponentForce.Clone();
               currentmoveForce.value = character.transform.TransformDirection(currentmoveForce.value);
            }
            else UnityEngine.Debug.Log("没有施加移动速度");

            while (tickCount++ <= number)
            {
                UnityEngine.Debug.Log("添加伤害");
                // 如果目标已被销毁，退出
                if (target == null || target.gameObject == null)
                    break;
                ApplyHitTo(currentchangeForce, currentmoveForce);
                yield return new WaitForSeconds(time / number);
               
            }
        }
        activeCoroutines.Remove(target);
    }

    /// <summary>
    /// 每次被攻击，导致改变角色当前的属性，从而改变角色的状态//xxx加上属性
    /// </summary>
    public void ApplyHitTo(AddVector3Temp hitchange, AddVector3Temp hitmove)
    {

        if (hitchange != null)
        {
            //添加防御属性等

            BuffMgr.Instance.AddBuff<Vector3>(passiveForce, hitchange.value,
            hitchange.time, hitchange.buffCurveType, hitchange.buffStackType, hitchange.buffOperationType);
        }

        if (hitmove != null)
        {
            //添加韧性属性等

            BuffMgr.Instance.AddBuff<Vector3>(passiveStackingMove, hitmove.value,
                       hitmove.time, hitmove.buffCurveType, hitmove.buffStackType, hitmove.buffOperationType);
        }

    }

    /// <summary>
    /// 实际伤害逻辑
    /// </summary>
    void ApplyDamageTo(AddVector3Temp change, AddVector3Temp move)
    {


    }


    //根据不同对象朝向施加速度
    bool isUsingOtherMoveForward = false;
    Transform targetMoveForward = null;
    public void ChangeCharacterMoveForward(Transform target, float time)
    {
        if (transform == target)
        {
            isUsingOtherMoveForward = false;
            targetMoveForward = null;


        }
        else
        {
            targetMoveForward = target;
            isUsingOtherMoveForward = true;
            if (coroutine_DoChangeCharacterMoveForward != null)
            {
                StopCoroutine(Do_ChangeCharacterMoveForward(time));
            }
            coroutine_DoChangeCharacterMoveForward = StartCoroutine(Do_ChangeCharacterMoveForward(time));
        }

    }
    Coroutine coroutine_DoChangeCharacterMoveForward;
    IEnumerator Do_ChangeCharacterMoveForward(float time)
    {
        var currenttime = Time.time;

        while (Time.time - currenttime < time)
        {
            yield return new WaitForEndOfFrame();
        }
        isUsingOtherMoveForward = false;
        coroutine_DoChangeCharacterMoveForward = null;
    }


    /// <summary>
    /// 调整角色的形状
    /// </summary>
    public void ChangeCharacterBox(E_CharacterBoxType e_CharacterBoxType)
    {
        if (e_CharacterBoxType == 0) return;
    }
    BoolTrigger isopenCollisionCheck = new BoolTrigger();
    void SetCanPassThrough(string[] layer, BoolTrigger boolTrigger)
    {
        int playerLayer = gameObject.layer;
        //int passThroughLayer = LayerMask.NameToLayer(layer);
        //Physics.IgnoreLayerCollision(playerLayer, passThroughLayer, boolTrigger.Value); 
        foreach (string layerName in layer)
        {
            int targetLayer = LayerMask.NameToLayer(layerName);
            if (targetLayer != -1)
            {
                Physics.IgnoreLayerCollision(playerLayer, targetLayer, boolTrigger.Value);
            }
        }
        boolTrigger.Value = true;
    }
    void SetHeight(float newHeight)
    {
        if (controller == null) return;

        if (!isopenCollisionCheck.Value)
        {
            // 1. 记录当前胶囊体底部世界位置
            Vector3 bottomWorldPos = transform.position + controller.center - Vector3.up * (controller.height / 2);

            // 2. 设置新高度
            controller.height = newHeight;

            // 3. 调整 center，使底部回到原位置
            // 新的 center 应该是：从 transform.position 到新胶囊体中心的偏移
            // 胶囊体中心 = bottomWorldPos + Vector3.up * (newHeight / 2)
            Vector3 newCenter = bottomWorldPos + Vector3.up * (newHeight / 2) - transform.position;
            controller.center = newCenter;
        }
        isopenCollisionCheck.Value = true;
    }


    /// <summary>
    /// 添加音效，离主摄像机多近才能听到
    /// </summary>
    /// <param name="mainCameraDistance"></param>
    public void ApplySoundEffects(Transform mainCameraDistance)
    {
        
    }


    /// <summary>
    /// 添加相机控制效果
    /// </summary>
    public void ApplyCameraEffect()
    {

    }


    //角色事件
    public void DO_Move_x(float x)
    {
        playerMove.x = x;
        if (playerMove.magnitude > 0.1f) _fsm.ActiveStateTrigger(E_StateEvent.移动);
    }
    public void DO_Move_y(float y)
    {
        playerMove.y = y;
        if(playerMove.magnitude > 0.1f) _fsm.ActiveStateTrigger(E_StateEvent.移动);
    }
    public void DO_Attack()
    {
        _fsm.ActiveStateTrigger(E_StateEvent.攻击);
       
    }
}