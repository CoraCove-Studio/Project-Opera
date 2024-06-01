using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;
using System.Net.Sockets;

using System;

using System.Text;

using BESimpleJSON;
using System.Diagnostics;


namespace BEngine {
    public static class BENetworking
    {
        #if UNITY_EDITOR

        private const int _firstMessgaeLength = 256;


		public static void RunBEServer() {
            BESettings beSettings = (BESettings)AssetDatabase.LoadMainAssetAtPath(BESettings.BEProjectSettingsPath);

            RunBlender.CheckBeforeRun(beSettings, null);

            if (beSettings.runBlenderType == RunBlenderType.RunNetwork) {
				var tmpFolder_Fixed = BEPaths.FixPath(beSettings.TMPFolderPath, true);
				var pyModulePath_Fixed = BEPaths.FixPath(beSettings.BEPythonModulePath, true);
				string pyFilePath = pyModulePath_Fixed + BECommon.BERunNodes_PY;

                // if pyFilePath exists
                if (File.Exists(pyFilePath) == false) {
                    // makers exception
                    string excStr = "Please, Check BEngine-Python Module Path in Project Settings!!! ";
                    excStr += BECommon.BERunNodes_PY + " file wasn't found in " + pyFilePath;
                    throw new Exception(excStr);
                



                }


				Process process = RunBlender.CreateBlenderProcess(pyFilePath, null, tmpFolder_Fixed, false);

				process.Start();
			} else {
				UnityEngine.Debug.Log("Please, Setup RunBlenderType as " + RunBlenderType.RunNetwork.ToString() + " in Project Settings!!!");
			}

		}


        public static void RunClient(BEngineComponent beComp, RunNodesType runType) {
            BESettings beSettings = (BESettings)AssetDatabase.LoadMainAssetAtPath(BESettings.BEProjectSettingsPath);

            TcpClient mySocket;

            mySocket = new TcpClient();
            // mySocket.ReceiveTimeout = 10000;
            // mySocket.SendTimeout = 10000;
            mySocket.ReceiveBufferSize = beSettings.MaxPackageBytes;

            RunSocket(mySocket, beSettings, beComp, runType);

            mySocket.Close();
        }


        public static bool RunSocket(TcpClient clientSocket, BESettings beSettings,
                                       BEngineComponent beComp, RunNodesType runType) {
            try
            {
                clientSocket.Connect(beSettings.Host, (int)beSettings.Port);

                BlenderPaths blenderPaths = BEPaths.GetBlenderPaths(beComp.BlenderParams);

                JSONNode jsDataOut = new JSONObject();

                var jsUnityBaseValues = BEJSONParser.GetUnityBaseValuesJSON(blenderPaths, beComp.BlenderParams, beSettings, runType);
                var jsUnityValues = BEJSONParser.GetUnityValuesJSON(beComp);

                jsDataOut["BaseValues"] = jsUnityBaseValues;
                
                if (runType == RunNodesType.RunNodes) {
                    jsDataOut["FromEngineData"] = jsUnityValues;
                }

                // Send
                byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(jsDataOut.ToString());
                DoSend(clientSocket, sendBytes);

                // Run Nodes
                if (runType == RunNodesType.RunNodes) {
                    // Receive
                    var receivedData = DoReceive(clientSocket, beSettings.MaxPackageBytes);

                    clientSocket.Close();
                    clientSocket.Dispose();

                    JSONNode jsDataIn = JSONNode.Parse(receivedData);

                    // Load Blender Outputs
                    BEJSONParser.LoadBlenderOutputs(beComp, jsDataIn);
                }
                
                else {
                    clientSocket.Close();
                    clientSocket.Dispose();
                }

                return true;
            }

            catch (Exception e)
            {
                UnityEngine.Debug.Log("Socket error: " + e);
                return false;
            }
        }


        public static void DoSend(TcpClient clientSocket, byte[] sendBytes){
            byte[] firstMessage = System.Text.Encoding.UTF8.GetBytes(sendBytes.Length.ToString());
            byte[] secondMessage = System.Text.Encoding.UTF8.GetBytes("0");  // Is Pickle = False. For BEngine Only.

            clientSocket.GetStream().Write(firstMessage, 0, firstMessage.Length);  // Send Length of a final message
            clientSocket.GetStream().Read(new byte[_firstMessgaeLength], 0, _firstMessgaeLength);  // Only for sending a final message
            
            clientSocket.GetStream().Write(secondMessage, 0, secondMessage.Length);  // Send Length of a final message
            clientSocket.GetStream().Read(new byte[_firstMessgaeLength], 0, _firstMessgaeLength);  // Only for sending a final message

            // Send Final Data
            clientSocket.GetStream().Write(sendBytes, 0, sendBytes.Length);
        }


        public static string DoReceive(TcpClient clientSocket, int maxPackageBytes) {
            var firstMessageBytes = new byte[_firstMessgaeLength];

            var finalMessageLengthBytes = clientSocket.GetStream().Read(firstMessageBytes, 0, _firstMessgaeLength);
            int finalMessageLength = Int32.Parse(Encoding.UTF8.GetString(firstMessageBytes));

            // Send the same bytes so that to get a final message
            clientSocket.GetStream().Write(firstMessageBytes, 0, firstMessageBytes.Length);

            // IsPickle for BEngine. Passed in Unity.
            firstMessageBytes = new byte[_firstMessgaeLength];
            var secondMessageBytes = clientSocket.GetStream().Read(firstMessageBytes, 0, _firstMessgaeLength);
            clientSocket.GetStream().Write(firstMessageBytes, 0, firstMessageBytes.Length);

            // Final Message
            string finalMessage = RecvAll(clientSocket, maxPackageBytes, finalMessageLength);

            return finalMessage;
        }


        // Receive Big Data
        public static string RecvAll(TcpClient clientSocket, int maxPackageBytes, int messageLength) {
            var memStream = new MemoryStream();
            var receiveData = new byte[maxPackageBytes];

            while (true)
            {
                var xLen = clientSocket.GetStream().Read(receiveData, 0, clientSocket.ReceiveBufferSize);

                // if (xLen == 0) {
                //     break;
                // }

                memStream.Write(receiveData, 0, xLen);

                if (memStream.Length >= messageLength) {
                    break;
                }
            }

            string receivedData = Encoding.UTF8.GetString(memStream.ToArray());

            memStream.Dispose();

            return receivedData;
        }


        #endif
    }
}
