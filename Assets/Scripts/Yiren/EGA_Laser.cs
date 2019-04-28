using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class EGA_Laser : MonoBehaviour
{
    public GameObject EffectBig;
    public GameObject EffectSmall;
    public float HitOffset = 0;

    public float MaxLength;
    private LineRenderer Laser;

    public float MainTextureLength = 1f;
    public float NoiseTextureLength = 1f;
    private Vector4 Length = new Vector4(1,1,1,1);
    private Vector4 LaserSpeed = new Vector4(0, 0, 0, 0);
    private Vector4 LaserStartSpeed;
    //One activation per shoot
    private bool LaserSaver = false;

    private ParticleSystem[] Effects;
    
    private ParticleSystem[] Hit;
    private Transform player;
    private Transform otherPlayer;
    public bool isConnecting = true;
    public bool isWithdrawing = false;
    public bool isDisconnecting = false;
    public bool isSwapping = false;
    public bool isMoveTowardBig = false;
    private Vector3 endPos;
    private Vector3 startPos;
    
    private Vector3 endPosSmall;
    private Vector3 startPosSmall;
    
    void Start ()
    {
        //Get LineRender and ParticleSystem components from current prefab;  
        Laser = GetComponent<LineRenderer>();
        Effects = GetComponentsInChildren<ParticleSystem>();
        Hit = EffectBig.GetComponentsInChildren<ParticleSystem>();
        if (Laser.material.HasProperty("_SpeedMainTexUVNoiseZW")) LaserStartSpeed = Laser.material.GetVector("_SpeedMainTexUVNoiseZW");
        //Save [1] and [3] textures speed
        LaserSpeed = LaserStartSpeed;
        player = GameObject.FindGameObjectWithTag("Player2").transform;
        otherPlayer = GameObject.FindGameObjectWithTag("Player1").transform;
    }

    void Update()
    {
        if (isSwapping)
        {
            //move small
            
            startPos =  Vector3.MoveTowards(startPos, playerCenterPos(), 260f * Time.deltaTime);           
            Laser.SetPosition(0, startPos);
            EffectSmall.transform.position = startPos;
            EffectSmall.transform.rotation = Quaternion.identity;
            //move big
            endPos = Vector3.MoveTowards(endPos, otherPlayerCenterPos(), 260f * Time.deltaTime);
            Laser.SetPosition(1, endPos);            
            EffectBig.transform.position = endPos;
            EffectBig.transform.rotation = Quaternion.identity;
            
            
            foreach (var AllPs in Effects)
            {
                if (!AllPs.isPlaying) AllPs.Play();
            }
            
            if (Vector3.Distance(startPos, playerCenterPos()) < 0.1f)
            {
                foreach (var AllPs in Effects)
                {
                    AllPs.Stop();
                }

                isSwapping = false;
                isConnecting = true;

            }
        }
        else if (isConnecting)
        {
            if (Laser.material.HasProperty("_SpeedMainTexUVNoiseZW")) Laser.material.SetVector("_SpeedMainTexUVNoiseZW", LaserSpeed);
        //SetVector("_TilingMainTexUVNoiseZW", Length); - old code, _TilingMainTexUVNoiseZW no more exist
        Laser.material.SetTextureScale("_MainTex", new Vector2(Length[0], Length[1]));                    
        Laser.material.SetTextureScale("_Noise", new Vector2(Length[2], Length[3]));
        //To set LineRender position
        if (Laser != null)
        {
            startPos = playerCenterPos();
            Laser.SetPosition(0, startPos);
            endPos = otherPlayerCenterPos();
            Laser.SetPosition(1, endPos);
            EffectBig.transform.position = endPos;
            foreach (var AllPs in Hit)
            {
                if (AllPs.isPlaying) AllPs.Stop();
            }
            //Texture tiling
            Length[0] = MainTextureLength * (Vector3.Distance(startPos, endPos));
            Length[2] = NoiseTextureLength * (Vector3.Distance(startPos, endPos));
            LaserSpeed[0] = (LaserStartSpeed[0] * 4) / (Vector3.Distance(startPos, endPos));
            LaserSpeed[2] = (LaserStartSpeed[2] * 4) / (Vector3.Distance(startPos, endPos));
           
            if (Laser.enabled == false && LaserSaver == false)
            {
                
                LaserSaver = true;
                Laser.enabled = true;
            }
        }  
        }
        else if(isWithdrawing)
        {
            startPos = playerCenterPos();
            Laser.SetPosition(0, startPos);
            endPos = Vector3.MoveTowards(endPos, startPos, 160f * Time.deltaTime);
            Laser.SetPosition(1, endPos);
            EffectBig.transform.position = endPos;
            //Hit effect zero rotation
            EffectBig.transform.rotation = Quaternion.identity;
            foreach (var AllPs in Hit)
            {
                if (!AllPs.isPlaying) AllPs.Play();
            }
            if (Vector3.Distance(endPos, startPos) < 0.1f)
            {
                foreach (var AllPs in Effects)
                {
                    AllPs.Stop();
                }
                isWithdrawing = false;
                
            }
        }
        else if (isMoveTowardBig)
        {
            startPos = playerCenterPos();
            Laser.SetPosition(0, startPos);
            endPos = Vector3.MoveTowards(endPos, otherPlayerCenterPos(), 160f * Time.deltaTime);
            Laser.SetPosition(1, endPos);
            EffectBig.transform.position = endPos;
            //Hit effect zero rotation
            EffectBig.transform.rotation = Quaternion.identity;
            foreach (var AllPs in Hit)
            {
                if (!AllPs.isPlaying) AllPs.Play();
            }
            if (Vector3.Distance(endPos, otherPlayerCenterPos()) < 0.1f)
            {
                foreach (var AllPs in Effects)
                {
                    AllPs.Stop();
                }

                isMoveTowardBig = false;
                isConnecting = true;

            }
        }
    }

    private Vector3 playerCenterPos()
    {
        return player.position + new Vector3(0, player.GetComponent<Collider>().bounds.extents.y, 0);
    }

    private Vector3 otherPlayerCenterPos()
    {
        Transform transmitter = otherPlayer.GetComponent<Ability_ThrowTransmitter>().TeleportTransmitter;
        if (transmitter.gameObject.activeSelf)
        {
            return transmitter.position;
        }
        return otherPlayer.position + new Vector3(0, otherPlayer.GetComponent<Collider>().bounds.extents.y, 0);
    }
    

    public void DisablePrepare()
    {
        if (Laser != null)
        {
            Laser.enabled = false;
        }
        //Effects can = null in multiply shooting
        if (Effects != null)
        {
            foreach (var AllPs in Effects)
            {
                if (AllPs.isPlaying) AllPs.Stop();
            }
        }

        isConnecting = false;
    }

    public void Disconnect()
    {
        if (!isConnecting || isWithdrawing)
        {
            return;
        }
        isConnecting = false;
        isWithdrawing = true;
        isMoveTowardBig = false;
    }

    public void Connect()
    {
        if (isConnecting || isMoveTowardBig || isSwapping)
        {
            return;
        }

        isWithdrawing = false;
        isMoveTowardBig = true;
        startPos = playerCenterPos();
        endPos = startPos;
    }
    
    public void Swap()
    {
        if (isSwapping)
        {
            return;
        }

        isWithdrawing = false;
        isMoveTowardBig = false;
        isConnecting = false;
        isSwapping = true;
        EffectSmall.transform.position = otherPlayerCenterPos();
    }
}
