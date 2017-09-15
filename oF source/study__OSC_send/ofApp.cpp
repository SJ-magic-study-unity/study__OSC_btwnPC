/************************************************************
************************************************************/
#include "ofApp.h"

/************************************************************
************************************************************/
//--------------------------------------------------------------
void ofApp::setup(){
	/********************
	********************/
	ofSetWindowTitle( "OSC receive" );
	ofSetWindowShape( WIDTH, HEIGHT );
	
	ofSetVerticalSync(true);
	ofSetFrameRate(30);
	ofSetEscapeQuitsApp(false);
	
	/********************
	********************/
	receiver.setup(PORT);
}

//--------------------------------------------------------------
void ofApp::update(){

	bool b_GetMousePos = false;
	
	while(receiver.hasWaitingMessages()){
		ofxOscMessage m;
		receiver.getNextMessage(m);
		
		if(m.getAddress() == "/test"){
			/********************
			Unityで複数のparameterを同時に送るには、型が同じである必要があるようだ.
			********************/
			/*
			int idt = m.getArgAsInt32(0);
			float fdt = m.getArgAsFloat(1);
			string sdt = m.getArgAsString(2);
			
			printf("\nOSC:%d, %f, %s\n", idt, fdt, sdt.c_str());
			*/
			
			int idt[3];
			for(int i = 0; i < 3; i++){
				idt[i] = m.getArgAsInt32(i);
			}
			
			printf("\nOSC:%d, %d, %d\n", idt[0], idt[1], idt[2]);
			
		}else if(m.getAddress() == "/mouse"){
			mouseX = m.getArgAsInt32(0);
			mouseY = m.getArgAsInt32(1);
			
			printf("(% 6d, % 6d)\r", mouseX, mouseY);
			fflush(stdout);
			
			b_GetMousePos = true;
		}
	}
	
	if(!b_GetMousePos){
		printf("(------, ------)\r");
		fflush(stdout);
	}
}

//--------------------------------------------------------------
void ofApp::draw(){
	ofBackground(30);
}

//--------------------------------------------------------------
void ofApp::keyPressed(int key){

}

//--------------------------------------------------------------
void ofApp::keyReleased(int key){

}

//--------------------------------------------------------------
void ofApp::mouseMoved(int x, int y ){

}

//--------------------------------------------------------------
void ofApp::mouseDragged(int x, int y, int button){

}

//--------------------------------------------------------------
void ofApp::mousePressed(int x, int y, int button){

}

//--------------------------------------------------------------
void ofApp::mouseReleased(int x, int y, int button){

}

//--------------------------------------------------------------
void ofApp::mouseEntered(int x, int y){

}

//--------------------------------------------------------------
void ofApp::mouseExited(int x, int y){

}

//--------------------------------------------------------------
void ofApp::windowResized(int w, int h){

}

//--------------------------------------------------------------
void ofApp::gotMessage(ofMessage msg){

}

//--------------------------------------------------------------
void ofApp::dragEvent(ofDragInfo dragInfo){ 

}
