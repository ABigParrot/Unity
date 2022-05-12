 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SMBTiming { OnEnter, OnExit, OnUpdate, OnEnd }
public class SMB_Event : StateMachineBehaviour
{
    [System.Serializable]
    public class SMBEvent
    {
        public bool fired;
        public string eventName;
        public SMBTiming timing;
        public float onUpdateFrame = 1;
    }

    [SerializeField] public int m_totalFrames;
    [SerializeField] public int m_currentFrame;
    [SerializeField] public float m_normalizedTime;
    [SerializeField] public float m_normalizedTimeUncapped;
    [SerializeField] public string m_motionTime = "";

    public List<SMBEvent> Events = new List<SMBEvent>();

    private bool m_hasParam;
    private Comp_SMBEventCurrator m_eventCurator;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_hasParam = HasParameter(animator, m_motionTime);
        m_eventCurator = animator.GetComponent<Comp_SMBEventCurrator>();
        m_totalFrames = GetTotalFrames(animator, layerIndex);

        m_normalizedTimeUncapped = stateInfo.normalizedTime;
        m_normalizedTime = m_hasParam ? animator.GetFloat(m_motionTime) : GetNormalizedTime(stateInfo);
        m_currentFrame = GetCurrentFrame(m_totalFrames, m_normalizedTime);

        //If the event curator isn't null...
        if (m_eventCurator != null)
            //... for every state machine behavior event in the SMBEvents list...
            foreach (SMBEvent _smbEvent in Events)
            {
                //... none of them are fired by default. But...
                _smbEvent.fired = false;

                //... if the timing of the event is equal to the entry timing of the state machine behavior...
                if (_smbEvent.timing == SMBTiming.OnEnter)
                    //... the event is fired and the curator invokes the event by its name.
                    _smbEvent.fired = true;
                    m_eventCurator.Event.Invoke(_smbEvent.eventName);
                
            }
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //If the event curator isn't null...
        if (m_eventCurator != null)
            //... for every state machine behavior event in the event list
            foreach (SMBEvent _smbEvent in Events)
            {
                //... if the event isn't fired...
                if (!_smbEvent.fired)
                {
                    // ... if the event timing is equal to the update timing of the state machine behavior ...
                    if (_smbEvent.timing == SMBTiming.OnUpdate)
                    {
                        //... if the current frame is less than or equal to the update frame of the event ...  
                        if (m_currentFrame >= _smbEvent.onUpdateFrame)
                        {
                            //... the event is fired and the curator invokes the event by its name.
                            _smbEvent.fired = true;
                            m_eventCurator.Event.Invoke(_smbEvent.eventName);
                        }
                    }
                }
                //Alternatively, if the event timing is equal to the state machine behaviors end timing...
                else if (_smbEvent.timing == SMBTiming.OnEnd)
                {
                    //...if the current frame is les than or equal to the total frames
                    if (m_currentFrame >= m_totalFrames)
                    {
                        //... the event is fired and the curator invokes the event by its name.
                        _smbEvent.fired = true; 
                        m_eventCurator.Event.Invoke(_smbEvent.eventName);
                    }
                    
                }
            }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_eventCurator != null)
        {
            foreach (SMBEvent _smbEvent in Events)
            {
                if (_smbEvent.timing == SMBTiming.OnExit)
                {
                     //... the event is fired and the curator invokes the event by its name.
                     _smbEvent.fired = true; 
                     m_eventCurator.Event.Invoke(_smbEvent.eventName);
                }
            }
        }
    }

    public bool HasParameter(Animator animator, string parameterName)
    {
        if (string.IsNullOrEmpty(parameterName) || string.IsNullOrWhiteSpace(parameterName))
            return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
            if (parameter.name == parameterName)
                return true;
        
        return false;
    }
    private int GetTotalFrames(Animator animator, int layerIndex)
    {
        AnimatorClipInfo[] _clipInfos = animator.GetNextAnimatorClipInfo(layerIndex);
        if (_clipInfos.Length == 0)
            _clipInfos = animator.GetCurrentAnimatorClipInfo(layerIndex);
        AnimationClip _clip = _clipInfos[0].clip;
        return Mathf.RoundToInt(_clip.length * _clip.frameRate);
    }
    private float GetNormalizedTime(AnimatorStateInfo stateInfo)
    {
        return stateInfo.normalizedTime > 1 ? 1 : stateInfo.normalizedTime;
    }
    private int GetCurrentFrame(int totalFrames, float normalizedTime)
    {
        return Mathf.RoundToInt(totalFrames * normalizedTime);
    }
}
