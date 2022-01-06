using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;



	//定数////////////////////////////////////////////////////////////////////


	//Pubic Field////////////////////////////////////////////////////////////////////
    [SerializeField , Range(10,100)]
    int resolution = 10;
	[SerializeField]
	FunctionLibrary.FunctionName functionName;

	float duration;
	[SerializeField, Min(0f)]
	float functionExistingDuration = 1f, transitionDuration = 1f;

	public enum TransitionMode { Cycle, Random}

	[SerializeField]
	TransitionMode transitionMode ;



	//Private Field////////////////////////////////////////////////////////////////////

	Transform[] points;

	bool isTransitioning;
	FunctionLibrary.FunctionName transitionFunctionName;



	//仮想メソッド定義////////////////////////////////////////////////////////////////////



	//Unityメソッド定義////////////////////////////////////////////////////////////////////

    void Awake()
    {

        float step = 2f/resolution;
		var position = Vector3.zero;
		var scale = Vector3.one * step;
		points = new Transform[resolution*resolution];
		for (int i = 0; i < points.Length; i++) {
			Transform point = points[i] = Instantiate(pointPrefab); //右から順に処理
            //point がインスタンス化されたObjを保有している（points[i] を代入したため）
			point.SetParent(transform, false);
			//point.localPosition = Vector3.right * ((i + 0.5f) / 5f - 1f);
			position.x = (i + 0.5f) * step - 1f; //規格化

			point.localPosition = position; //生成するObjの値の設定
			point.localScale = scale;
		}

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
		if (isTransitioning) {
			UpdateFunctionTransition();
		}else{
			UpdateFunction();
		}
	}


	

	//publicメソッド定義////////////////////////////////////////////////////////////////////


	//privateメソッド定義////////////////////////////////////////////////////////////////////
	void UpdateFunction() {
		FunctionLibrary.Function f = FunctionLibrary.GetFunction(functionName);
		float time = Time.time;
		float step = 2f / resolution;
		float v = 0.5f * step - 1f;
		for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++) {
			if (x == resolution) {
				x = 0;
				z += 1;
				v = (z + 0.5f) * step - 1f;
			}
			float u = (x + 0.5f) * step - 1f;
			points[i].localPosition = f(u, v, time);
			// ｆはu,v という二次元平面を代入してその平面に対するｙ座標を新たに加えたVec3として返す
			//ｘ、ｚが横平面
		}
	}


	void UpdateFunctionTransition(){
		FunctionLibrary.Function f = FunctionLibrary.GetFunction(functionName);

		FunctionLibrary.Function 
			from = FunctionLibrary.GetFunction(transitionFunctionName),
			to  = FunctionLibrary.GetFunction(functionName);
		float progress = duration / transitionDuration;
		float time = Time.time;
		float step = 2f / resolution;
		float v = .5f * step -1f;
		for (int i = 0, x = 0, z = 0; i < points.Length; i++,x++) {
			if (x == resolution) {
				x = 0;
				z += 1;
				v = (z + 0.5f) * step - 1f;
			}
			float u = (x + 0.5f) * step - 1f;
			points[i].localPosition = FunctionLibrary.Morph(u,v,time, from,to, progress);
		}

	}

	void PickNextFunction() {
		functionName = transitionMode == TransitionMode.Cycle ?
			FunctionLibrary.GetNextFnName(functionName) :
			FunctionLibrary.GetOtherRandomFnName(functionName);
	}


}
