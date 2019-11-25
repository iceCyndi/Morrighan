using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mogo.Util;
using Mogo.Game;
using Mogo.GameData;
using System;
using System.IO;
using System.Text;

public class ActorMyself : ActorPlayer<EntityMyself>
{
    private const string EXPORT_FILE_PATH = "Assets/Resources/data/xml";
    private const string EXPORT_FILE_NAME = "GearData.xml";
    public bool isMoving = false;
    public MogoMotor mogoMotor;
    private float preSpeed;
    
    private float powerTime = 0;
    private bool powering = false;
    private bool chargstart = false;
    private string cmds = "";

    void Start()
    {
        gameObject.layer = 8;
        DontDestroyOnLoad(this);
        if (isNeedInitEquip) InitEquipment();
        m_billboardTrans = transform.FindChild("slot_billboard");
    }
    void Update()
    {
        ActChange();
        //ProcessMotionInput();

        if (theEntity == null)
            return;
        if (m_billboardTrans != null && theEntity != null)
        {
            // ...
        }
    }
    void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {

        }
        if (Input.GetButton("1"))
        {
            //theEntity.SpellO;
        }
        else if (Input.GetButton("2"))
        { }
        else if (Input.GetButton("3"))
        {
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {

        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {

        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            float tt = Time.realtimeSinceStartup;
            if (tt - powerTime > 0.5f && !chargstart)
            {
                chargstart = true;
                theEntity.PowerChargeStart();
            }
        }
        else
        {
            if (chargstart)
            {
                chargstart = false;
                theEntity.PowerChargeInterrupt();
            }
            powering = false;
        }
        if (MogoWorld.showClientGM)
        { }
    }
    void ProcessMotionInput()
    {
        if (theEntity == null)
            return;

        if (theEntity.deathFlag == 1 || theEntity.currSpellID > 0)
        {
            if (ControlStick.instance.IsDraging)
            {
                //(theEntity is EntityMyself).ClearCmdCache();
            }
            return;
        }
        if (mogoMotor.enableStick)
        {
            if (ControlStick.instance.IsDraging)
            {
                isMoving = true;
               // theEntity.Move();
            }
            else if (!mogoMotor.isMovingToTarget)
            {
                if (isMoving)
                {
                    isMoving = false;
                    theEntity.Idle();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
        }
        else if (Input.GetKeyDown(KeyCode.Menu))
        {

        }
        else if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            //theEntity.CreateDuplication();
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {

        }
        else if (Input.GetKeyDown(KeyCode.KeypadMultiply))
        {

        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {

        }
        else if (Input.GetKeyDown(KeyCode.F6))
        {

        }
        else if (Input.GetKeyDown(KeyCode.F7))
        {

        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {

        }
        else if (Input.GetKeyDown(KeyCode.F8))
        {

        }
        else if (Input.GetKeyDown(KeyCode.F12))
        {

        }
        else if (Input.GetKeyDown(KeyCode.F11))
        {

        }
        else if (Input.GetKeyDown(KeyCode.F9))
        {

        }
        else if (Input.GetKeyDown(KeyCode.F10))
        {

        }
        else if (Input.GetKeyDown(KeyCode.Home))
        {

        }
    }
    public void SetControl(bool curIsMovable)
    {
        mogoMotor.iSMovable = curIsMovable;
    }
    public void SetMoveTo(Vector3 target, bool bLookAtTarget = false)
    {

    }
    public void SetMoveToDirectly(Vector3 distance)
    {
        transform.position += distance;
    }
    public void SpeedDown(float coefficient)
    {
        if (mogoMotor.speed == preSpeed)
        {
            preSpeed = mogoMotor.speed;
            mogoMotor.SetSpeed(preSpeed * coefficient);
        }
    }
    public void SpeedUp() 
    {
        if (mogoMotor.speed != preSpeed)
        {
            mogoMotor.SetSpeed(preSpeed);
        }
    }
}