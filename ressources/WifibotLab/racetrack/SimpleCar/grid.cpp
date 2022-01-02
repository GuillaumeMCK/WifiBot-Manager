//Simple keyboard controlled car.
//Controls: arrow keys.

//Don't forget to see Reference.txt document for details on functions called in this example! 

//THIS SOURCE CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND. USE AT YOUR OWN RISK.

#include <windows.h>
#include <stdio.h>
#include <math.h>
#include "..\common\3impactDLL_defs.h"
#include "..\common\3impact_cameraman.h"

//declare object variables
class GRID{
 public:
  MESH * mesh;
  BODY * body;
  void on();
  bool run(void){  return true;  }
  void off(void);
  int unsigned id,gid,options,state;
  void renew(char bodyfile[], char meshfile[]);
  void setpos(float x, float y);
  GRID(char bodyfile[], char meshfile[]);
  GRID ();
  ~GRID ();
};

  void GRID::on(){
   iBodyEnable(body);
   iMeshShow(mesh);
  }
  
  void GRID::off(void){
   iMeshHide(mesh);
   iBodyDisable(body);
  }

  void GRID::renew(char bodyfile[], char meshfile[]){
   iBodyDestroy(body);
   iMeshDestroy(mesh);
   mesh=iMeshCreate(meshfile);
   body=iBodySGCreate(bodyfile,2.0f);
  }
  
  void GRID::setpos(float x, float y){
   iBodyLocationSet(body,&D3DXVECTOR3(x,y,0.0f),true);
   iMeshLocationSet(mesh,&D3DXVECTOR3(x,y,0.0f));
  }
  
  GRID::GRID(char bodyfile[], char meshfile[]){
   body=iBodySGCreate(bodyfile,2.0f);
   mesh=iMeshCreate(meshfile);
   id=0;
   gid=0;
   options=0;
   state=0;
  }
  
  GRID::GRID(void){
   body=iBodySGCreate("default_res\\ball_.spg",2.0f);
   mesh=iMeshCreate("default_res\\floorplate1.x");  
   id=0;
   gid=0;
   options=0;
   state=0;
  }
  
  GRID::~GRID () {
   off();
   iMeshDestroy(mesh);
   iBodyDestroy(body);
  }

class MouseLookCamera
{
public:
   CAMERA* maincam;
   
   MouseLookCamera(CAMERA* cam)
   {
       maincam=cam;
   }
   
   void Update(void)
   {
       float MouseX,MouseY;
       D3DXVECTOR2 MouseDirection(0,0);
       static D3DXVECTOR2 CamRotation;
       char TextString[100];
       MouseX=iMouseX();
       MouseY=iMouseY();
       if((MouseX == 0.5) && (MouseY == 0.5))  return;
       iMouseReset(&D3DXVECTOR2(0.5,0.5));
       MouseDirection.x = (0.5 - MouseX)*50;
       MouseDirection.y = (0.5 - MouseY)*50;
       CamRotation -= MouseDirection;
       iCameraEulerOrientationSet(maincam,CamRotation.y,CamRotation.x,0,"xyz");
   }
};


CAMERA *Camera;
SPRITE *LoadingMessage;
SKYBOX *SkyBox;
MESH *terra;
BODY *terrabody;
MouseLookCamera *mlc;



void PreInit(DWORD* settings)
{
}
void Init()
{


   //create and set camera
   Camera=iCameraCreate(0,0,100,100);
   iCameraFovSet(Camera,35.0f);
   mlc=new MouseLookCamera(Camera);
   
  //set sunlight
   iLightDirectionalSet(&D3DXVECTOR3(1.0f,-1.0f,1.0f),&D3DXVECTOR4(1.0f,1.0f,1.0f,0.35f));
   //create and show 'loading...' sprite
   LoadingMessage=iSpriteCreate("default_res\\sprites\\loading.x",NULL);
   iSpriteRender(LoadingMessage,NULL);
   //create static background
   SkyBox=iSkyBoxCreate("default_res\\skyboxes\\skybox01.sky");
   iSpriteHide(LoadingMessage);
   //show static background and all meshes
   iSkyBoxShow(SkyBox);
   
 //  terra=iMeshCreate("default_res\\terrain.x");
 // terrabody=iBodyCreate("default_res\\terrain_5.00.ply");
 //  iMeshShow(terra);
   D3DXVECTOR3 *pos=&D3DXVECTOR3(0.0f,0.0f,0.0f);
   GRID * grid[100];
   GRID *gpt;
   float y=0.0f;
   float x=0.0f;
   gpt=grid[0];
   for(int i = 0; i < 100; i++){
    gpt=new GRID("default_res\\ball_.spg","default_res\\floorplate1.x");
    gpt->on();
    gpt->setpos(x,y);
//    grid[i]=gpt;
    if (y<10.0f){
     y+=1.0f;
    } else {
     x+=1.0f;
     y=0.0f;
    }
    gpt++;
   }
   gpt--;
   iCameraLocationSet(Camera,&D3DXVECTOR3(-5.0f,40.2f,10.0f));
   iMeshLocation(gpt->mesh,pos);
   iCameraLookAt(Camera,pos,1.0f);
   
}
void Run()
{
mlc->Update();
}
void Exit()
{
}
void PostExit(HWND hWnd)
{
}

