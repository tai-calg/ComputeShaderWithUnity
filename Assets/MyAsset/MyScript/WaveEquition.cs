
using UnityEngine;

public class WaveEquition : MonoBehaviour
{

	//定数////////////////////////////////////////////////////////////////////



	//Pubic Field////////////////////////////////////////////////////////////////////



	//Private Field////////////////////////////////////////////////////////////////////


	[SerializeField] GameObject plane;
	[SerializeField] ComputeShader computeShader;
	[SerializeField] float deltaSize = 0.1f;
	[SerializeField] float waveCoef = 1.0f;

	private RenderTexture waveTexture, drawTexture;

	private int kernelInitialize, kernelUpdate, kernelDraw, kernelAddWave;
	private ThreadSize threadSizeInitialize, threadSizeUpdate, threadSizeDraw;

	struct ThreadSize {
		public int x, y, z;

		public ThreadSize(uint x, uint y, uint z) {
			this.x = (int)x;
			this.y = (int)y;
			this.z = (int)z;
		}
	}

	//Unityメソッド定義////////////////////////////////////////////////////////////////////

	private void Start(){
		// kernel id の取得
		kernelInitialize = computeShader.FindKernel("Initialize");
		kernelUpdate = computeShader.FindKernel("Update");
		kernelDraw = computeShader.FindKernel("Draw");
		kernelAddWave = computeShader.FindKernel("AddWave");

		//波の高さを格納するテクスチャの作成
		waveTexture = new RenderTexture(256, 256, 0, RenderTextureFormat.RG32);
		waveTexture.wrapMode = TextureWrapMode.Clamp;
		waveTexture.enableRandomWrite = true;
		waveTexture.Create();


		//波を描画するテクスチャの作成
		drawTexture = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
		drawTexture.enableRandomWrite = true;
		drawTexture.Create();

		//スレッド数の初期化
		uint threadSizeX, threadSizeY, threadSizeZ;
		computeShader.GetKernelThreadGroupSizes(kernelInitialize, out threadSizeX, out threadSizeY, out threadSizeZ);
		threadSizeInitialize = new ThreadSize(threadSizeX, threadSizeY, threadSizeZ);

		computeShader.GetKernelThreadGroupSizes(kernelUpdate, out threadSizeX, out threadSizeY, out threadSizeZ);
		threadSizeUpdate = new ThreadSize(threadSizeX, threadSizeY, threadSizeZ);

		computeShader.GetKernelThreadGroupSizes(kernelDraw, out threadSizeX, out threadSizeY, out threadSizeZ);
		threadSizeDraw = new ThreadSize(threadSizeX, threadSizeY, threadSizeZ);

		//波の初期化
		computeShader.SetTexture(kernelInitialize, "waveTexture", waveTexture);
		computeShader.Dispatch(kernelInitialize,Mathf.CeilToInt(waveTexture.width/ threadSizeInitialize.x),
			Mathf.CeilToInt(waveTexture.width / threadSizeInitialize.y), 
			1);

		

	}

	private void FixedUpdate()
	{
		this.computeShader.SetFloat("time", Time.time);
		this.computeShader.SetTexture(kernelAddWave, "waveTexture", waveTexture);
		this.computeShader.Dispatch(kernelAddWave, 
			Mathf.CeilToInt(waveTexture.width / threadSizeUpdate.x),
			Mathf.CeilToInt(waveTexture.width / threadSizeUpdate.y),
			1);

		//波の高さ更新
		this.computeShader.SetFloat("deltaSize", deltaSize);
		this.computeShader.SetFloat("waveCoef", waveCoef);
		this.computeShader.SetFloat("deltaTime", Time.deltaTime* 2.0f);
		this.computeShader.SetTexture(kernelUpdate, "waveTexture", waveTexture);
		this.computeShader.Dispatch(kernelUpdate,
			Mathf.CeilToInt(waveTexture.width / threadSizeUpdate.x),
			Mathf.CeilToInt(waveTexture.width / threadSizeUpdate.y),
			1);

		//波の高さをもとにレンダリング用のテクスチャを作成
		this.computeShader.SetTexture(kernelDraw, "waveTexture", waveTexture);
		this.computeShader.SetTexture(kernelDraw, "drawTexture", drawTexture);//ここはORマスクしてるのでは。前Updateと今回のAddした分を。
		this.computeShader.Dispatch(kernelDraw,
			Mathf.CeilToInt(waveTexture.width / threadSizeDraw.x),
			Mathf.CeilToInt(waveTexture.width / threadSizeDraw.y),
			1);

		plane.GetComponent<Renderer>().material.mainTexture = drawTexture;
	}

	//publicメソッド定義////////////////////////////////////////////////////////////////////



	//privateメソッド定義////////////////////////////////////////////////////////////////////


}
