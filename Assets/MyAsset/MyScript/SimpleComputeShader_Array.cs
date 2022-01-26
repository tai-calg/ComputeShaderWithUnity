using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

コンピュ―トシェーダーへの参照
実行するカーネルのインデックス
コンピュ―トシェーダーの実行結果を格納するバッファ
*/
public class SimpleComputeShader_Array : MonoBehaviour
{

	//定数////////////////////////////////////////////////////////////////////



	//Pubic Field////////////////////////////////////////////////////////////////////
	public ComputeShader computeShader;




	//Private Field////////////////////////////////////////////////////////////////////
	int kernelIndex_KernelFunction_A;
	int kernelIndex_KernelFunction_B;
	ComputeBuffer intComputerBuffer;



	//Unityメソッド定義////////////////////////////////////////////////////////////////////
	void Start()
	{
		this.kernelIndex_KernelFunction_A = this.computeShader.FindKernel("KernelFunction_A");
		this.kernelIndex_KernelFunction_B = this.computeShader.FindKernel("KernelFunction_B");

		this.intComputerBuffer = new ComputeBuffer(4, sizeof(int));
		this.computeShader.SetBuffer(
			this.kernelIndex_KernelFunction_A,
			"intBuffer",
			this.intComputerBuffer
		);

		this.computeShader.SetInt("intValue", 1);

		//Dispatch	で.computeを実行
		this.computeShader.Dispatch(
			this.kernelIndex_KernelFunction_A,
			1,
			1,
			1
		);
		
		int[] result = new int[4];
		this.intComputerBuffer.GetData(result);
		for (int i = 0; i < 4 ; i++){
			Debug.Log(result[i]);
		}

		this.computeShader.SetBuffer(
			this.kernelIndex_KernelFunction_B,
			"intBuffer",
			this.intComputerBuffer
		);
		
		this.computeShader.Dispatch(this.kernelIndex_KernelFunction_B, 1, 1, 1);

		this.intComputerBuffer.GetData(result);

		Debug.Log("result : KernelFunction_B");

		for (int i = 0; i < 4 ; i++){
			Debug.Log(result[i]);
		}

		//バッファを解放
		this.intComputerBuffer.Release();


	}



	//publicメソッド定義////////////////////////////////////////////////////////////////////



	//privateメソッド定義////////////////////////////////////////////////////////////////////


}
