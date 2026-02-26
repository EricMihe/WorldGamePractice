using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using static UnityEditor.Progress;
using static UnityEditor.Sprites.Packer;
using static UnityEngine.EventSystems.EventTrigger;
public interface BaseFSM
{
    BaseCharacter Character { get; set; }
    StateEntity CurrentState { get; set; }
    void Open();
    void Run();
    void Close();
    void ChangeState(StateEntity state);
    void ActiveStateTrigger(E_StateEvent e_StateEvent);
    void Init(BaseCharacter character);
}

public class FSM:BaseFSM
{
    BaseCharacter _character; 
    StateEntity _currentstate;
    bool isopen=false;
    public BaseCharacter Character { get => _character; set => _character =value ; }
    public StateEntity CurrentState { get => _currentstate; set => _currentstate=value ; }
    List<StateEntity> _stateEntities=new List<StateEntity>();
    List<StateEntity> _passiveStateEntities = new List<StateEntity>();
    List<StateEntity> _selfpassiveStateEntities = new List<StateEntity>();
    MaxPriorityQueue<StateEntity> waittingState= new MaxPriorityQueue<StateEntity>((a, b) => a.statusPriority.CompareTo(b.statusPriority));
    UnityAction passiveStateDO;
    UnityAction selfpassiveStateDO;
    UnityAction<E_StateEvent> activeStateDO;
    public void Init(BaseCharacter character)
    {
        this._character = character;
        if (_character != null)
        {

            var stateEntities = _character.stateTableObject;
            if (stateEntities != null)
            {
                foreach (var entity in stateEntities.states)
                {
                    _stateEntities.Add(entity);
                }
            }
            _passiveStateEntities = _stateEntities.Where(e => e.type == E_StateType.被动状态).ToList();
            _selfpassiveStateEntities = _stateEntities.Where(e => e.type == E_StateType.自身被动状态).ToList();
        }
        Open();
    }
    public void ChangeState(StateEntity state)
    {

        StateExit();
        if (state != null&& _currentstate != null) Debug.Log($"切换状态：{_currentstate.statename}到{state.statename}");
        _currentstate = state;
        _character.animator.CrossFade(_currentstate.animClipName, crossFadeDuration, 0, 0f);
        StateEnter();

    }

    public void Close()
    {
        isopen = false;
        StateExit();

            passiveStateDO -= passiveStateChangeListen;
            selfpassiveStateDO -= selfpassiveStateChangeListen;
            activeStateDO -= activeStateChangeListen;
        
    }
    public void Open()
    {
        passiveStateDO += passiveStateChangeListen;
        selfpassiveStateDO += selfpassiveStateChangeListen;
        activeStateDO += activeStateChangeListen;
        var startstate = _stateEntities.Find(e => e.statename == "initialState"|| e.statename == "InitialState"||e.statename == "initialstate" || e.statename == "idle"||e.statename=="待机");
        if(startstate != null)
        {
            waittingState.Enqueue(startstate);
        }  
        else Debug.Log("没有配置初始状态");
        isopen = true;
    }

    StateEventConfig _currentStateEventConfig;
    StateEntity _nextstateEntity;
    StateEntity stateEntity;
    public void Run()
    {   
        if(isopen)
        {
            if (_passiveStateEntities.Count > 0&& _character.currentpassiveForce.magnitude>0.01f) passiveStateDO.Invoke();

            if (_selfpassiveStateEntities.Count > 0 && _character.currentselfpassiveForce.magnitude > 0.01f) selfpassiveStateDO.Invoke();

            if( _character.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f >= 0.95f) ActiveStateTrigger(E_StateEvent.空闲);

            if (waittingState.Count > 0) _nextstateEntity = waittingState.Dequeue();

            if (_nextstateEntity != null)
            {
                if (!_character.animator.IsInTransition(0))
                {
                    ChangeState(_nextstateEntity);
                }
            }
            waittingState.Clear();
            _nextstateEntity = null;
        } 
    }


    public float crossFadeDuration = 0.05f; // 过渡时间（秒）
    Dictionary<int, int> addeffect = new Dictionary<int, int>();
    Dictionary<int, int> addeffectWithCollider = new Dictionary<int, int>();
    float _currentStateStartTime;
    float animClipLength;
    public void StateEnter()
    {
        Debug.Log("状态进入");
        Debug.Log($"{_character}");
        _currentStateStartTime = Time.time;
        animClipLength = _character.animator.GetCurrentAnimatorStateInfo(0).length;
        //if (_currentstate .type ==E_StateType.主动状态 )
        //{
           
        //    animClipLength = _character.animator.GetCurrentAnimatorStateInfo(0).length / _character.characterChangeSpeed;
        //    _character.animator.speed = _character.characterChangeSpeed;
        //}

        if (_currentstate.type ==E_StateType.被动状态 )
        {
            BuffMgr.Instance.ReMoveAllBuff<Vector3>(_character.passiveForce);
            BuffMgr.Instance.ReMoveAllBuff<Vector3>(_character.activeStackingMove);
            BuffMgr.Instance.ReMoveAllBuff<Vector3>(_character.selfpassiveStackingMove);
            if (_currentstate.addSpeedInStates.buffs.Count > 0 && Time.time - _currentStateStartTime < animClipLength)
            {
                BuffMgr.Instance.AddBuffWithDIY<Vector3>(_character.passiveStackingMove, _currentstate.addSpeedInStates.time == 0 ? animClipLength : _currentstate.addSpeedInStates.time,
            _currentstate.addSpeedInStates.buffs, _currentstate.addSpeedInStates.buffStackType, _currentstate.addSpeedInStates.buffOperationType);

            }

            // BuffMgr.Instance.AddBuff<float>(_character.currentResilienceName, _currentstate.imBanlance, animClipLength, BuffCurveType.LinearDecay );
        }
        else if (_currentstate.type == E_StateType.自身被动状态)
        {
            BuffMgr.Instance.ReMoveAllBuff<Vector3>(_character.selfpassiveForce);
            BuffMgr.Instance.ReMoveAllBuff<Vector3>(_character.activeStackingMove);
            if (_currentstate.addSpeedInStates.buffs.Count > 0 && Time.time - _currentStateStartTime < animClipLength )
            {
                BuffMgr.Instance.AddBuffWithDIY<Vector3>(_character.selfpassiveStackingMove, _currentstate.addSpeedInStates.time == 0 ? animClipLength : _currentstate.addSpeedInStates.time,
            _currentstate.addSpeedInStates.buffs, _currentstate.addSpeedInStates.buffStackType, _currentstate.addSpeedInStates.buffOperationType);

            }
            //BuffMgr.Instance.AddBuff<float>(_character.currentResilienceName, _currentstate.imBanlance, animClipLength, BuffCurveType.LinearDecay);

        }
        else
        {
            if (_currentstate.addSpeedInStates.buffs.Count > 0 && Time.time - _currentStateStartTime < animClipLength)
            {
                BuffMgr.Instance.AddBuffWithDIY<Vector3>(_character.activeStackingMove, _currentstate.addSpeedInStates.time == 0 ? animClipLength : _currentstate.addSpeedInStates.time,
            _currentstate.addSpeedInStates.buffs, _currentstate.addSpeedInStates.buffStackType, _currentstate.addSpeedInStates.buffOperationType);

            }
        }


        if (_currentstate.addEffectInStates.Count > 0 && Time.time - _currentStateStartTime < animClipLength)
        {
            for (int i = 0; i < _currentstate.addEffectInStates.Count; i++)
            {
                if (_currentstate.addEffectInStates[i].path != "" && _currentstate.addEffectInStates[i].endtime > 0)
                {
                    addeffect.Add(i, TimerMgr.Instance.CreateTimerWithParam<AddEffectInState>(_currentstate.addEffectInStates[i].starttime, AddEffectExcuteListen, _currentstate.addEffectInStates[i]));
                    addeffectWithCollider.Add(i, TimerMgr.Instance.CreateTimerWithParam2<AddEffectInState, bool>(_currentstate.addEffectInStates[i].triggerstarttime , AddEffectExcuteWithColliderListen, _currentstate.addEffectInStates[i], true));
                }
                else if (_currentstate.addEffectInStates[i].triggerendtime > 0)
                {
                    addeffectWithCollider.Add(i, TimerMgr.Instance.CreateTimerWithParam2<AddEffectInState, bool>(_currentstate.addEffectInStates[i].triggerstarttime, AddEffectExcuteWithColliderListen, _currentstate.addEffectInStates[i], false));

                }
            }
        }
        AddAttribute();
    }



    public void StateExit()
    {
        Debug.Log("状态退出");
        _character.animator.speed = 1f;
        _currentStateStartTime = 0f;

        //animClipLength = 0f;
        foreach (var i in addeffect)
        {
            TimerMgr.Instance.RemoveTimer(i.Value);
        }
        foreach (var i in addeffectWithCollider)
        {
            TimerMgr.Instance.RemoveTimer(i.Value);
        }
        addeffect.Clear();
        addeffectWithCollider.Clear();
        RemoveAttribute();
        _character.CloseAllEffectCollider();
    }
    private void AddAttribute()
    {

        BuffMgr.Instance.AddBuff<float>(_character.moveSpeedScale, _currentstate.stateMoveForce.value,
                        _currentstate.stateMoveForce.time ==0  ? animClipLength  : _currentstate.stateMoveForce.time , _currentstate.stateMoveForce.buffCurveType,
                        _currentstate.stateMoveForce.buffStackType, _currentstate.stateMoveForce.buffOperationType);

        BuffMgr.Instance.AddBuff<float>(_character.moveSpeedScaler, _currentstate.stateMoveForcer.value,
            _currentstate.stateMoveForcer.time == 0? animClipLength : _currentstate.stateMoveForcer.time, _currentstate.stateMoveForcer.buffCurveType,
            _currentstate.stateMoveForcer.buffStackType, _currentstate.stateMoveForcer.buffOperationType);


        BuffMgr.Instance.AddBuff<float>(_character.passiveMoveSpeedScale, _currentstate.statePassiveMoveForce.value,
            _currentstate.statePassiveMoveForce.time == 0 ? animClipLength: _currentstate.statePassiveMoveForce.time, _currentstate.statePassiveMoveForce.buffCurveType,
            _currentstate.statePassiveMoveForce.buffStackType, _currentstate.statePassiveMoveForce.buffOperationType);

        BuffMgr.Instance.AddBuff<float>(_character.passiveMoveSpeedScaler, _currentstate.statePassiveMoveForcer.value,
            _currentstate.statePassiveMoveForcer.time == 0 ? animClipLength  : _currentstate.statePassiveMoveForcer.time, _currentstate.statePassiveMoveForcer.buffCurveType,
            _currentstate.statePassiveMoveForcer.buffStackType, _currentstate.statePassiveMoveForcer.buffOperationType);
        

        BuffMgr.Instance.AddBuff<float>(_character.currentResilience, _currentstate.extraStiffness, _currentstate.extraStiffnesstime== 0 ? animClipLength : _currentstate.extraStiffnesstime);

       

    }
    private void RemoveAttribute()
    {

        BuffMgr.Instance.ReMoveAllBuff<float>(_character.moveSpeedScale);
        BuffMgr.Instance.ReMoveAllBuff<float>(_character.moveSpeedScaler);

        BuffMgr.Instance.ReMoveAllBuff<float>(_character.passiveMoveSpeedScale);

        BuffMgr.Instance.ReMoveAllBuff<float>(_character.passiveMoveSpeedScaler);

        BuffMgr.Instance.ReMoveAllBuff<float>(_character.currentResilience);
    }

    void passiveStateChangeListen()
    {
        foreach (var entity in _passiveStateEntities)
        {
            if (Vector3.Distance(_character.currentpassiveForce, Vector3.zero) >=  (entity.imBanlance<0.05f? 0.05f: entity.imBanlance) + _character.currenResilience
                && Vector3.Angle(_character.currentpassiveForce, entity.directionChangeForce) < 100f)
            {
                if (!entity.isWait &&  _character.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f >=
                      1f - Mathf.Clamp01(_currentstate.stateChangeTime) + (1f - Mathf.Clamp01(_currentstate.stateChangeTime)) * Mathf.Clamp01(Vector2.Distance(entity.stateChangeForce, _currentstate.stateChangeForce)) - 0.05f)
                {
                    waittingState.Enqueue(entity);
                }
                else if (!entity.isWait && (entity.statusPriority >= _currentstate.statusPriority|| _currentstate.type == E_StateType.自身被动状态 || _currentstate.type == E_StateType.主动状态))
                {
                    waittingState.Enqueue(entity);
                }
                else if (_character.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f >= 0.95f)
                {
                    waittingState.Enqueue(entity);
                }
            }
        }
    }

    void selfpassiveStateChangeListen()
    {

        foreach (var entity in _selfpassiveStateEntities)
        {
            if (Vector3.Distance(_character.currentselfpassiveForce , Vector3.zero) >=  (entity.imBanlance < 0.05f ? 0.05f : entity.imBanlance)
                && Vector3.Angle(_character.currentpassiveForce, entity.directionChangeForce) < 100f)
            {
                if (!entity.isWait &&  _character.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f >=
                      1f - Mathf.Clamp01(_currentstate.stateChangeTime) + (1f - Mathf.Clamp01(_currentstate.stateChangeTime)) * Mathf.Clamp01(Vector2.Distance(entity.stateChangeForce, _currentstate.stateChangeForce)) - 0.05f)
                {
                    waittingState.Enqueue(entity);
                }
                else if(!entity.isWait && ((entity.statusPriority >= _currentstate.statusPriority && _currentstate.type !=E_StateType.被动状态) || _currentstate.type == E_StateType.主动状态))
                {
                    waittingState.Enqueue(entity);
                }
                else if (_character.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f >= 0.95f)
                {
                    waittingState.Enqueue(entity);
                }
            }

        }
    }

    void activeStateChangeListen(E_StateEvent e_StateEvent)
    {

        if (_currentstate!= null)
        {
            _currentStateEventConfig = _currentstate.stateEventConfigs.Find(e => e.e_StateEvent == e_StateEvent);

        }
        if (_currentStateEventConfig != null)
        {
            stateEntity = _stateEntities.Find(e => e.statename == _currentStateEventConfig.toStateEntityname);
            if (stateEntity != null)
            {
                if (!stateEntity.isWait &&  _character.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f >=
                      1f - Mathf.Clamp01(_currentstate.stateChangeTime) + (1f - Mathf.Clamp01(_currentstate.stateChangeTime)) * Mathf.Clamp01(Vector2.Distance(stateEntity.stateChangeForce, _currentstate.stateChangeForce)) - 0.05f)
                {
                    waittingState.Enqueue(stateEntity);
                    stateEntity = null;
                }
                else if(!stateEntity.isWait && stateEntity.statusPriority > _currentstate.statusPriority&& (_currentstate.type!=E_StateType.被动状态|| _currentstate.type != E_StateType.自身被动状态))
                {
                    waittingState.Enqueue(stateEntity);
                    stateEntity = null;
                }
                else if (_character.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f >= 0.95f)
                {
                    waittingState.Enqueue(stateEntity);
                    stateEntity = null;
                }
            }
        }
    }


    public AddVector3Temp currentchangeForce;
    public AddVector3Temp currentmoveForce;

    void AddEffectExcuteListen(AddEffectInState addEffectInState)
    {  
        //_character.ApplyEffect(addEffectInState, false, (addEffectInState.endtime - addEffectInState.starttime) / animClipSpeed);

        _character.AddEffcetOnTriggerPiont(addEffectInState.endtime - addEffectInState.starttime, addEffectInState.path, addEffectInState.piont);
        
    }

    void AddEffectExcuteWithColliderListen(AddEffectInState addEffectInState,bool isUisngObj)
    {
        if (isUisngObj)
        {
            if (addEffectInState.isOpenCheck && addEffectInState.triggerendtime > 0)
            {
                _character.OpenTriggerWithAddEffectCollider(addEffectInState.stateChangeOpponentForce, addEffectInState.stateMoveOpponentForce, addEffectInState.effectType, addEffectInState.path, addEffectInState.piont, addEffectInState.triggerendtime - addEffectInState.triggerstarttime, addEffectInState.triggernumber);

            }
        }
        else
        {
            _character.OpenTriggerPiontCollider(addEffectInState.stateChangeOpponentForce, addEffectInState.stateMoveOpponentForce, addEffectInState.effectType, addEffectInState.piont, addEffectInState.triggerendtime - addEffectInState.triggerstarttime, addEffectInState.triggernumber);

        }
        //_character.ApplyEffect(addEffectInState, true, (addEffectInState.triggerendtime - addEffectInState.triggerstarttime) / animClipSpeed, addEffectInState.triggernumber, _currentstate.isAddCharacterAttributes);

    }

    public void ActiveStateTrigger(E_StateEvent e_StateEvent)
    {
        Debug.Log($"{e_StateEvent}触发");
        activeStateDO.Invoke(e_StateEvent);
    }
}
