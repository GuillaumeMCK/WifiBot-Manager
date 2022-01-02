//Simple keyboard controlled car.
//Controls: arrow keys.

//Don't forget to see Reference.txt document for details on functions called in this example! 

//THIS SOURCE CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND. USE AT YOUR OWN RISK.

#include <windows.h>
#include <stdio.h>
#include <math.h>
#include "..\common\3impactDLL_defs.h"

//declare object variables
CAMERA* Camera;
SPRITE* LoadingMessage;
SKYBOX* SkyBox;
BODY* Terrain;
MESH* TerrainMesh;
BODY* CarBody;
MESH* CarBodyMesh;
MESH* CarBodyMeshShadow;
BODY* CarWheelFL;
MESH* CarWheelFLMesh;
MESH* CarWheelFLMeshShadow;
BODY* CarWheelFR;
MESH* CarWheelFRMesh;
MESH* CarWheelFRMeshShadow;
BODY* CarWheelRL;
MESH* CarWheelRLMesh;
MESH* CarWheelRLMeshShadow;
BODY* CarWheelRR;
MESH* CarWheelRRMesh;
MESH* CarWheelRRMeshShadow;
BODYBODY* CarBodyTerrain;
BODYBODY* CarWheelFLTerrain;
BODYBODY* CarWheelFRTerrain;
BODYBODY* CarWheelRLTerrain;
BODYBODY* CarWheelRRTerrain;
WHEEL* CarWheelFLJoint;
WHEEL* CarWheelFRJoint;
WHEEL* CarWheelRLJoint;
WHEEL* CarWheelRRJoint;
//declare parameter variables
float SideFriction;
float RollingWiseFriction;
float SuspensionSpring;
float SuspensionDamping;
float SteerAngle;
float Spin;
float WheelSpin;
float WheelPower;
float begin;

void PreInit(DWORD* settings)
{
}
void Init()
{


begin = 1.0f;


}
void Run()
{

if(begin == 1.0f) { 

  //------------------------------
   //create and set camera
   Camera=iCameraCreate(0,0,100,100);
   iCameraFovSet(Camera,35.0f);
   iCameraLocationSet(Camera,&D3DXVECTOR3(23.0f,10.0f,-100.0f));
   //set sunlight
   iLightDirectionalSet(&D3DXVECTOR3(1.0f,-1.0f,1.0f),&D3DXVECTOR4(1.0f,1.0f,1.0f,0.35f));
   //create and show 'loading...' sprite
   LoadingMessage=iSpriteCreate("default_res\\sprites\\loading.x",NULL);
   iSpriteRender(LoadingMessage,NULL);
   //create static background
   SkyBox=iSkyBoxCreate("default_res\\skyboxes\\skybox01.sky");
   //create terrain static body and mesh
   Terrain=iBodyCreate("default_res\\terrain_5.00.ply");
   TerrainMesh=iBodyMeshCreate("default_res\\terrain.x",Terrain);

   //create dynamic car-body body and mesh
   CarBody=iBodySGCreate("simplecar_res\\carbody_.spg",0.13f);
   CarBodyMesh=iBodyMeshCreate("simplecar_res\\carbody.x",CarBody);
   //set initial car position
   iBodyLocationSet(CarBody,&D3DXVECTOR3(43.0f,5.0f,-100.0f),TRUE);
   //a little bit of damping is always good to prevent excessive speed and spin
   iBodyDampingSet(CarBody,0.0f,0.999f,0.0f,0.995f);
   //create car-body shadow mesh
   CarBodyMeshShadow=iMeshBodyShadowCasterCreate("simplecar_res\\carbody_shadow.x",CarBody,500.0f);

   //enable car-terrain collision checking (create collision couple)
   CarBodyTerrain=iBodyBodyCreate(CarBody,Terrain);
   iBodyBodyFrictionSet(CarBodyTerrain,2.5f);

   //********************************************************
   //SIMULATION PARAMETERS: you can adjust car-dynamics here!

   //the friction produced by wheels, when they move sideways respect to terrain
   SideFriction = 7.0f;
   //the friction produced by wheels, when they move parallel to rolling direction
   RollingWiseFriction = 3.0f;
   //suspension strength
   SuspensionSpring = 10.0f;
   //suspension hardness
   SuspensionDamping = 1.0f;
   //maximum wheel spin (degrees per second)
   WheelSpin = 1600.0f;
   //maximum wheel power applied to reach and keep maximum speed
   WheelPower = 1.0f;

   //********************************************************

   //initialize steering and throttle
   SteerAngle = 0.0f;
   Spin = 0.0f;

   //create FRONT-LEFT wheel dynamic body, mesh and shadow mesh
   CarWheelFL=iBodySGCreate("simplecar_res\\carwheel_.spg",0.13f);
   CarWheelFLMesh=iBodyMeshCreate("simplecar_res\\carwheel.x",CarWheelFL);
   CarWheelFLMeshShadow=iMeshBodyShadowCasterCreate("simplecar_res\\carwheel_shadow.x",CarWheelFL,500.0f);
   //enable terrain-wheel collision checking
   CarWheelFLTerrain=iBodyBodyCreate(CarWheelFL,Terrain);
   //enable split-friction mode
   iBodyBodySplitFrictionSet(CarWheelFLTerrain,SideFriction,0.0f,RollingWiseFriction,0.0f);
   //create wheel joint (attach wheel-body to car-body) and set suspension parameters
   CarWheelFLJoint = iWheelCreate(CarBody,CarWheelFL,&D3DXVECTOR3(-1.12f,0.16f,1.22f),NULL);
   iWheelSuspensionSet(CarWheelFLJoint,SuspensionSpring,SuspensionDamping);

   //create FRONT-RIGHT wheel dynamic body, mesh and shadow mesh
   CarWheelFR=iBodySGCreate("simplecar_res\\carwheel_.spg",0.13f);
   CarWheelFRMesh=iBodyMeshCreate("simplecar_res\\carwheel.x",CarWheelFR);
   CarWheelFRMeshShadow=iMeshBodyShadowCasterCreate("simplecar_res\\carwheel_shadow.x",CarWheelFR,500.0f);
   //enable terrain-wheel collision checking
   CarWheelFRTerrain=iBodyBodyCreate(CarWheelFR,Terrain);
   //enable split-friction mode
   iBodyBodySplitFrictionSet(CarWheelFRTerrain,SideFriction,0.0f,RollingWiseFriction,0.0f);
   //create wheel joint (attach wheel-body to car-body) and set suspension parameters
   CarWheelFRJoint = iWheelCreate(CarBody,CarWheelFR,&D3DXVECTOR3(1.12f,0.16f,1.22f),NULL);
   iWheelSuspensionSet(CarWheelFRJoint,SuspensionSpring,SuspensionDamping);

   //create REAR-LEFT wheel dynamic body, mesh and shadow mesh
   CarWheelRL=iBodySGCreate("simplecar_res\\carwheel_.spg",0.13f);
   CarWheelRLMesh=iBodyMeshCreate("simplecar_res\\carwheel.x",CarWheelRL);
   CarWheelRLMeshShadow=iMeshBodyShadowCasterCreate("simplecar_res\\carwheel_shadow.x",CarWheelRL,500.0f);
   //enable terrain-wheel collision checking
   CarWheelRLTerrain=iBodyBodyCreate(CarWheelRL,Terrain);
   //enable split-friction mode
   iBodyBodySplitFrictionSet(CarWheelRLTerrain,SideFriction,0.0f,RollingWiseFriction,0.0f);
   //create wheel joint (attach wheel-body to car-body) and set suspension parameters
   CarWheelRLJoint = iWheelCreate(CarBody,CarWheelRL,&D3DXVECTOR3(-1.12f,0.16f,-1.10f),NULL);
   iWheelSuspensionSet(CarWheelRLJoint,SuspensionSpring,SuspensionDamping);

   //create REAR-RIGHT wheel dynamic body, mesh and shadow mesh
   CarWheelRR=iBodySGCreate("simplecar_res\\carwheel_.spg",0.13f);
   CarWheelRRMesh=iBodyMeshCreate("simplecar_res\\carwheel.x",CarWheelRR);
   CarWheelRRMeshShadow=iMeshBodyShadowCasterCreate("simplecar_res\\carwheel_shadow.x",CarWheelRR,500.0f);
   //enable terrain-wheel collision checking
   CarWheelRRTerrain=iBodyBodyCreate(CarWheelRR,Terrain);
   //enable split-friction mode
   iBodyBodySplitFrictionSet(CarWheelRRTerrain,SideFriction,0.0f,RollingWiseFriction,0.0f);
   //create wheel joint (attach wheel-body to car-body) and set suspension parameters
   CarWheelRRJoint = iWheelCreate(CarBody,CarWheelRR,&D3DXVECTOR3(1.12f,0.16f,-1.10f),NULL);
   iWheelSuspensionSet(CarWheelRRJoint,SuspensionSpring,SuspensionDamping);

   //enable all bodies
   iBodyEnable(Terrain);
   iBodyEnable(CarBody);
   iBodyEnable(CarWheelFL);
   iBodyEnable(CarWheelFR);
   iBodyEnable(CarWheelRL);
   iBodyEnable(CarWheelRR);
   //hide 'loading...' sprite
   iSpriteHide(LoadingMessage);
   //show static background and all meshes
   iSkyBoxShow(SkyBox);
   iMeshShow(TerrainMesh);
   iMeshShow(CarBodyMesh);
   iMeshShow(CarBodyMeshShadow);
   iMeshShow(CarWheelFLMesh);
   iMeshShow(CarWheelFLMeshShadow);
   iMeshShow(CarWheelFRMesh);
   iMeshShow(CarWheelFRMeshShadow);
   iMeshShow(CarWheelRLMesh);
   iMeshShow(CarWheelRLMeshShadow);
   iMeshShow(CarWheelRRMesh);
   iMeshShow(CarWheelRRMeshShadow);   //create and set camera
   Camera=iCameraCreate(0,0,100,100);
   iCameraFovSet(Camera,35.0f);
   iCameraLocationSet(Camera,&D3DXVECTOR3(23.0f,10.0f,-100.0f));
   //set sunlight
   iLightDirectionalSet(&D3DXVECTOR3(1.0f,-1.0f,1.0f),&D3DXVECTOR4(1.0f,1.0f,1.0f,0.35f));
   //create and show 'loading...' sprite
   LoadingMessage=iSpriteCreate("default_res\\sprites\\loading.x",NULL);
   iSpriteRender(LoadingMessage,NULL);
   //create static background
   SkyBox=iSkyBoxCreate("default_res\\skyboxes\\skybox01.sky");
   //create terrain static body and mesh
   Terrain=iBodyCreate("default_res\\terrain_5.00.ply");
   TerrainMesh=iBodyMeshCreate("default_res\\terrain.x",Terrain);

   //create dynamic car-body body and mesh
   CarBody=iBodySGCreate("simplecar_res\\carbody_.spg",0.13f);
   CarBodyMesh=iBodyMeshCreate("simplecar_res\\carbody.x",CarBody);
   //set initial car position
   iBodyLocationSet(CarBody,&D3DXVECTOR3(43.0f,5.0f,-100.0f),TRUE);
   //a little bit of damping is always good to prevent excessive speed and spin
   iBodyDampingSet(CarBody,0.0f,0.999f,0.0f,0.995f);
   //create car-body shadow mesh
   CarBodyMeshShadow=iMeshBodyShadowCasterCreate("simplecar_res\\carbody_shadow.x",CarBody,500.0f);

   //enable car-terrain collision checking (create collision couple)
   CarBodyTerrain=iBodyBodyCreate(CarBody,Terrain);
   iBodyBodyFrictionSet(CarBodyTerrain,2.5f);

   //********************************************************
   //SIMULATION PARAMETERS: you can adjust car-dynamics here!

   //the friction produced by wheels, when they move sideways respect to terrain
   SideFriction = 7.0f;
   //the friction produced by wheels, when they move parallel to rolling direction
   RollingWiseFriction = 3.0f;
   //suspension strength
   SuspensionSpring = 10.0f;
   //suspension hardness
   SuspensionDamping = 1.0f;
   //maximum wheel spin (degrees per second)
   WheelSpin = 1600.0f;
   //maximum wheel power applied to reach and keep maximum speed
   WheelPower = 1.0f;

   //********************************************************

   //initialize steering and throttle
   SteerAngle = 0.0f;
   Spin = 0.0f;

   //create FRONT-LEFT wheel dynamic body, mesh and shadow mesh
   CarWheelFL=iBodySGCreate("simplecar_res\\carwheel_.spg",0.13f);
   CarWheelFLMesh=iBodyMeshCreate("simplecar_res\\carwheel.x",CarWheelFL);
   CarWheelFLMeshShadow=iMeshBodyShadowCasterCreate("simplecar_res\\carwheel_shadow.x",CarWheelFL,500.0f);
   //enable terrain-wheel collision checking
   CarWheelFLTerrain=iBodyBodyCreate(CarWheelFL,Terrain);
   //enable split-friction mode
   iBodyBodySplitFrictionSet(CarWheelFLTerrain,SideFriction,0.0f,RollingWiseFriction,0.0f);
   //create wheel joint (attach wheel-body to car-body) and set suspension parameters
   CarWheelFLJoint = iWheelCreate(CarBody,CarWheelFL,&D3DXVECTOR3(-1.12f,0.16f,1.22f),NULL);
   iWheelSuspensionSet(CarWheelFLJoint,SuspensionSpring,SuspensionDamping);

   //create FRONT-RIGHT wheel dynamic body, mesh and shadow mesh
   CarWheelFR=iBodySGCreate("simplecar_res\\carwheel_.spg",0.13f);
   CarWheelFRMesh=iBodyMeshCreate("simplecar_res\\carwheel.x",CarWheelFR);
   CarWheelFRMeshShadow=iMeshBodyShadowCasterCreate("simplecar_res\\carwheel_shadow.x",CarWheelFR,500.0f);
   //enable terrain-wheel collision checking
   CarWheelFRTerrain=iBodyBodyCreate(CarWheelFR,Terrain);
   //enable split-friction mode
   iBodyBodySplitFrictionSet(CarWheelFRTerrain,SideFriction,0.0f,RollingWiseFriction,0.0f);
   //create wheel joint (attach wheel-body to car-body) and set suspension parameters
   CarWheelFRJoint = iWheelCreate(CarBody,CarWheelFR,&D3DXVECTOR3(1.12f,0.16f,1.22f),NULL);
   iWheelSuspensionSet(CarWheelFRJoint,SuspensionSpring,SuspensionDamping);

   //create REAR-LEFT wheel dynamic body, mesh and shadow mesh
   CarWheelRL=iBodySGCreate("simplecar_res\\carwheel_.spg",0.13f);
   CarWheelRLMesh=iBodyMeshCreate("simplecar_res\\carwheel.x",CarWheelRL);
   CarWheelRLMeshShadow=iMeshBodyShadowCasterCreate("simplecar_res\\carwheel_shadow.x",CarWheelRL,500.0f);
   //enable terrain-wheel collision checking
   CarWheelRLTerrain=iBodyBodyCreate(CarWheelRL,Terrain);
   //enable split-friction mode
   iBodyBodySplitFrictionSet(CarWheelRLTerrain,SideFriction,0.0f,RollingWiseFriction,0.0f);
   //create wheel joint (attach wheel-body to car-body) and set suspension parameters
   CarWheelRLJoint = iWheelCreate(CarBody,CarWheelRL,&D3DXVECTOR3(-1.12f,0.16f,-1.10f),NULL);
   iWheelSuspensionSet(CarWheelRLJoint,SuspensionSpring,SuspensionDamping);

   //create REAR-RIGHT wheel dynamic body, mesh and shadow mesh
   CarWheelRR=iBodySGCreate("simplecar_res\\carwheel_.spg",0.13f);
   CarWheelRRMesh=iBodyMeshCreate("simplecar_res\\carwheel.x",CarWheelRR);
   CarWheelRRMeshShadow=iMeshBodyShadowCasterCreate("simplecar_res\\carwheel_shadow.x",CarWheelRR,500.0f);
   //enable terrain-wheel collision checking
   CarWheelRRTerrain=iBodyBodyCreate(CarWheelRR,Terrain);
   //enable split-friction mode
   iBodyBodySplitFrictionSet(CarWheelRRTerrain,SideFriction,0.0f,RollingWiseFriction,0.0f);
   //create wheel joint (attach wheel-body to car-body) and set suspension parameters
   CarWheelRRJoint = iWheelCreate(CarBody,CarWheelRR,&D3DXVECTOR3(1.12f,0.16f,-1.10f),NULL);
   iWheelSuspensionSet(CarWheelRRJoint,SuspensionSpring,SuspensionDamping);

   //enable all bodies
   iBodyEnable(Terrain);
   iBodyEnable(CarBody);
   iBodyEnable(CarWheelFL);
   iBodyEnable(CarWheelFR);
   iBodyEnable(CarWheelRL);
   iBodyEnable(CarWheelRR);
   //hide 'loading...' sprite
   iSpriteHide(LoadingMessage);
   //show static background and all meshes
   iSkyBoxShow(SkyBox);
   iMeshShow(TerrainMesh);
   iMeshShow(CarBodyMesh);
   iMeshShow(CarBodyMeshShadow);
   iMeshShow(CarWheelFLMesh);
   iMeshShow(CarWheelFLMeshShadow);
   iMeshShow(CarWheelFRMesh);
   iMeshShow(CarWheelFRMeshShadow);
   iMeshShow(CarWheelRLMesh);
   iMeshShow(CarWheelRLMeshShadow);
   iMeshShow(CarWheelRRMesh);
   iMeshShow(CarWheelRRMeshShadow);
   //------------------------------


begin = 0.0f;
}



   //update camera orientation so that it always takes the car
   D3DXVECTOR3 cameratarget;
   iBodyLocationCM(CarBody,&cameratarget);
   iCameraLookAt(Camera,&cameratarget,0.1f);

   //if left/right arrow keys are pressed, progressively increase/decrease the SteerAngle
   //variable (within -20 / 20 degrees).
   //If neither left nor right keys are pressed, progressively adjust SteerAngle variable
   //toward zero, in order to perform steering auto-centering.
   if (iKeyDown(DIK_LEFT))
   {
      iFloatTendTo(&SteerAngle,20.0f,0.3f);
   }
   else if (iKeyDown(DIK_RIGHT))
   {
      iFloatTendTo(&SteerAngle,-20.0f,0.3f);
   }
   else iFloatTendTo(&SteerAngle,0.0f,0.2f);
   //apply current SteerAngle to front wheels (set wheel joints steering parameter)
   iWheelSteeringAngleSet(CarWheelFLJoint,SteerAngle);
   iWheelSteeringAngleSet(CarWheelFRJoint,SteerAngle);

   //if up/down arrow keys are pressed, progressively decrease/increase the Spin
   //variable (within -WheelSpin / WheelSpin).
   //If neither up nor down keys are pressed, progressively adjust Spin variable
   //toward zero, in order to perform throttle auto-returning
   if (iKeyDown(DIK_UP))
   {
      iFloatTendTo(&Spin,-WheelSpin,0.4f);
   }
   else if (iKeyDown(DIK_DOWN))
   {
      iFloatTendTo(&Spin,WheelSpin,0.4f);
   }
   else iFloatTendTo(&Spin,0.0f,0.1f);

   //wheel power is controlled by two factors: the maximum spin permitted and the torque
   //intensity used to reach and keep such speed.
   //In order to simulate a simple one-gear system, in which wheel spin and power are
   //proportional to throttle depression, we let the user directly tune the
   //maximum spin with up/down arrow keys (previous section above).
   //Then we compute the actual wheel power (torque) so that it is proportional to
   //such user-adjusted Spin
   float power = iFloatInterpolate(iFloatAbs(Spin),0.0f,WheelSpin,WheelPower*0.1f,WheelPower,FALSE);
   //apply user-adjusted spin and computed power to all 4 wheels
   iWheelSpinSet(CarWheelFLJoint,Spin);
   iWheelSpinSet(CarWheelFRJoint,Spin);
   iWheelSpinSet(CarWheelRLJoint,Spin);
   iWheelSpinSet(CarWheelRRJoint,Spin);
   iWheelPowerSet(CarWheelFLJoint,power);
   iWheelPowerSet(CarWheelFRJoint,power);
   iWheelPowerSet(CarWheelRLJoint,power);
   iWheelPowerSet(CarWheelRRJoint,power);

   //apply gravity acceleration to car-body and wheel-bodies
   iBodyAccelerationApply(CarBody,&D3DXVECTOR3(0.0f,-9.80665f,0.0f));
   iBodyAccelerationApply(CarWheelFL,&D3DXVECTOR3(0.0f,-9.80665f,0.0f));
   iBodyAccelerationApply(CarWheelFR,&D3DXVECTOR3(0.0f,-9.80665f,0.0f));
   iBodyAccelerationApply(CarWheelRL,&D3DXVECTOR3(0.0f,-9.80665f,0.0f));
   iBodyAccelerationApply(CarWheelRR,&D3DXVECTOR3(0.0f,-9.80665f,0.0f));
}
void Exit()
{
}
void PostExit(HWND hWnd)
{
}





