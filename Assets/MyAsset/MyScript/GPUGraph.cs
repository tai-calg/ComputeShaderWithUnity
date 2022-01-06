using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public class GPUGraph : MonoBehaviour
{


	//定数////////////////////////////////////////////////////////////////////

	const int maxResolution = 1000;
	static readonly int positionId = Shader.PropertyToID("_Positions"),
	resolutionId = Shader.PropertyToID("_Resolution"),
	timeId = Shader.PropertyToID("_Time"),
	stepId = Shader.PropertyToID("_Step"),
	transitionProgressId = Shader.PropertyToID("_TransitionProgress");

	//Pubic Field////////////////////////////////////////////////////////////////////
    [SerializeField , Range(10,maxResolution)]
    int resolution = 10;
	[SerializeField]
	FunctionLibrary.FunctionName functionName;

	float duration;
	[SerializeField, Min(0f)]
	float functionExistingDuration = 1f, transitionDuration = 1f;

	public enum TransitionMode { Cycle, Random}

	[SerializeField]
	TransitionMode transitionMode ;

	[SerializeField]
	ComputeShader computeShader;

	[SerializeField]
	Material material;

	[SerializeField]
	Mesh mesh;



	//Private Field////////////////////////////////////////////////////////////////////


	bool isTransitioning;
	FunctionLibrary.FunctionName transitionFunctionName;

	ComputeBuffer positionBuffer;



	//仮想メソッド定義////////////////////////////////////////////////////////////////////



	//Unityメソッド定義////////////////////////////////////////////////////////////////////


	private void Awake()
	{
		positionBuffer = new ComputeBuffer(maxResolution*maxResolution, 3*4); //4byte * vector3;
	}

	private void OnDisable()
	{
		positionBuffer.Release();
		positionBuffer = null; // 明示的にnullにしておくことでGCに任せないでメモリ圧迫を回避。
	}

	void Update(){
		duration += Time.deltaTime;

		if (isTransitioning){
			if (duration >= transitionDuration){
				duration -= transitionDuration;
				isTransitioning = false;
			}
		}
		else if(duration >= functionExistingDuration) {
			duration -= functionExistingDuration;
			isTransitioning = true;
			transitionFunctionName = functionName;
			PickNextFunction();
		}

		UpdateFunctionOnGPU();

	}


	

	//publicメソッド定義////////////////////////////////////////////////////////////////////


	//privateメソッド定義////////////////////////////////////////////////////////////////////


	void UpdateFunctionOnGPU(){ //CPUで制御、GPUではCPUで命令したことを計算させるだけ。
		float step = 2f/resolution;
		computeShader.SetInt(resolutionId, resolution);
		computeShader.SetFloat(stepId, step);
		computeShader.SetFloat(timeId, Time.time);

		if (isTransitioning) {
			computeShader.SetFloat(
				transitionProgressId,
				Mathf.SmoothStep(0f,1f, duration / transitionDuration)
			);
		}

		var kernelIndex = 
					(int)functionName +
			(int)(isTransitioning ? transitionFunctionName : functionName) * FunctionLibrary.FunctionCount;
		

		computeShader.SetBuffer(kernelIndex, positionId, positionBuffer);
		int groups = Mathf.CeilToInt(resolution / 8f);
		computeShader.Dispatch(kernelIndex, groups, groups , 1);

		material.SetBuffer(positionId,positionBuffer);
		material.SetFloat(stepId, step);
		var bounds = new Bounds(Vector3.zero, Vector3.one *(2f + 2f / resolution));
		//バウンディングボックスのおかげで、カメラ外のメッシュ、マテリアルは描画されないようになる



		//最後にレンダリング
		Graphics.DrawMeshInstancedProcedural(mesh, 0 , material, bounds, resolution * resolution);

	}



	void PickNextFunction() {
		functionName = transitionMode == TransitionMode.Cycle ?
			FunctionLibrary.GetNextFnName(functionName) :
			FunctionLibrary.GetOtherRandomFnName(functionName);
	}


}
