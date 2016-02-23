using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match
{
	/// <summary>
	///   <para>A component for communicating with the UNET Matchmaking service.</para>
	/// </summary>
	public class NetworkMatch : MonoBehaviour
	{
		public delegate void ResponseDelegate<T>(T response);

		private const string kMultiplayerNetworkingIdKey = "CloudNetworkingId";

		private Uri m_BaseUri = new Uri("https://mm.unet.unity3d.com");

		/// <summary>
		///   <para>The base URI of the UNET MatchMaker that this NetworkMatch will communicate with.</para>
		/// </summary>
		public Uri baseUri
		{
			get
			{
				return this.m_BaseUri;
			}
			set
			{
				this.m_BaseUri = value;
			}
		}

		/// <summary>
		///   <para>Set this before calling any UNET functions. Must match AppID for this program from the Cloud API.</para>
		/// </summary>
		/// <param name="programAppID">AppID that corresponds to the Cloud API AppID for this app.</param>
		public void SetProgramAppID(AppID programAppID)
		{
			Utility.SetAppID(programAppID);
		}

		public Coroutine CreateMatch(string matchName, uint matchSize, bool matchAdvertise, string matchPassword, NetworkMatch.ResponseDelegate<CreateMatchResponse> callback)
		{
			return this.CreateMatch(new CreateMatchRequest
			{
				name = matchName,
				size = matchSize,
				advertise = matchAdvertise,
				password = matchPassword
			}, callback);
		}

		public Coroutine CreateMatch(CreateMatchRequest req, NetworkMatch.ResponseDelegate<CreateMatchResponse> callback)
		{
			Uri uri = new Uri(this.baseUri, "/json/reply/CreateMatchRequest");
			Debug.Log("MatchMakingClient Create :" + uri);
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("projectId", Application.cloudProjectId);
			wWWForm.AddField("sourceId", Utility.GetSourceID().ToString());
			wWWForm.AddField("appId", Utility.GetAppID().ToString());
			wWWForm.AddField("accessTokenString", 0);
			wWWForm.AddField("domain", 0);
			wWWForm.AddField("name", req.name);
			wWWForm.AddField("size", req.size.ToString());
			wWWForm.AddField("advertise", req.advertise.ToString());
			wWWForm.AddField("password", req.password);
			wWWForm.headers["Accept"] = "application/json";
			WWW client = new WWW(uri.ToString(), wWWForm);
			return base.StartCoroutine(this.ProcessMatchResponse<CreateMatchResponse>(client, callback));
		}

		public Coroutine JoinMatch(NetworkID netId, string matchPassword, NetworkMatch.ResponseDelegate<JoinMatchResponse> callback)
		{
			return this.JoinMatch(new JoinMatchRequest
			{
				networkId = netId,
				password = matchPassword
			}, callback);
		}

		public Coroutine JoinMatch(JoinMatchRequest req, NetworkMatch.ResponseDelegate<JoinMatchResponse> callback)
		{
			Uri uri = new Uri(this.baseUri, "/json/reply/JoinMatchRequest");
			Debug.Log("MatchMakingClient Join :" + uri);
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("projectId", Application.cloudProjectId);
			wWWForm.AddField("sourceId", Utility.GetSourceID().ToString());
			wWWForm.AddField("appId", Utility.GetAppID().ToString());
			wWWForm.AddField("accessTokenString", 0);
			wWWForm.AddField("domain", 0);
			wWWForm.AddField("networkId", req.networkId.ToString());
			wWWForm.AddField("password", req.password);
			wWWForm.headers["Accept"] = "application/json";
			WWW client = new WWW(uri.ToString(), wWWForm);
			return base.StartCoroutine(this.ProcessMatchResponse<JoinMatchResponse>(client, callback));
		}

		public Coroutine DestroyMatch(NetworkID netId, NetworkMatch.ResponseDelegate<BasicResponse> callback)
		{
			return this.DestroyMatch(new DestroyMatchRequest
			{
				networkId = netId
			}, callback);
		}

		public Coroutine DestroyMatch(DestroyMatchRequest req, NetworkMatch.ResponseDelegate<BasicResponse> callback)
		{
			Uri uri = new Uri(this.baseUri, "/json/reply/DestroyMatchRequest");
			Debug.Log("MatchMakingClient Destroy :" + uri.ToString());
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("projectId", Application.cloudProjectId);
			wWWForm.AddField("sourceId", Utility.GetSourceID().ToString());
			wWWForm.AddField("appId", Utility.GetAppID().ToString());
			wWWForm.AddField("accessTokenString", Utility.GetAccessTokenForNetwork(req.networkId).GetByteString());
			wWWForm.AddField("domain", 0);
			wWWForm.AddField("networkId", req.networkId.ToString());
			wWWForm.headers["Accept"] = "application/json";
			WWW client = new WWW(uri.ToString(), wWWForm);
			return base.StartCoroutine(this.ProcessMatchResponse<BasicResponse>(client, callback));
		}

		public Coroutine DropConnection(NetworkID netId, NodeID dropNodeId, NetworkMatch.ResponseDelegate<BasicResponse> callback)
		{
			return this.DropConnection(new DropConnectionRequest
			{
				networkId = netId,
				nodeId = dropNodeId
			}, callback);
		}

		public Coroutine DropConnection(DropConnectionRequest req, NetworkMatch.ResponseDelegate<BasicResponse> callback)
		{
			Uri uri = new Uri(this.baseUri, "/json/reply/DropConnectionRequest");
			Debug.Log("MatchMakingClient DropConnection :" + uri);
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("projectId", Application.cloudProjectId);
			wWWForm.AddField("sourceId", Utility.GetSourceID().ToString());
			wWWForm.AddField("appId", Utility.GetAppID().ToString());
			wWWForm.AddField("accessTokenString", Utility.GetAccessTokenForNetwork(req.networkId).GetByteString());
			wWWForm.AddField("domain", 0);
			wWWForm.AddField("networkId", req.networkId.ToString());
			wWWForm.AddField("nodeId", req.nodeId.ToString());
			wWWForm.headers["Accept"] = "application/json";
			WWW client = new WWW(uri.ToString(), wWWForm);
			return base.StartCoroutine(this.ProcessMatchResponse<BasicResponse>(client, callback));
		}

		public Coroutine ListMatches(int startPageNumber, int resultPageSize, string matchNameFilter, NetworkMatch.ResponseDelegate<ListMatchResponse> callback)
		{
			return this.ListMatches(new ListMatchRequest
			{
				pageNum = startPageNumber,
				pageSize = resultPageSize,
				nameFilter = matchNameFilter
			}, callback);
		}

		public Coroutine ListMatches(ListMatchRequest req, NetworkMatch.ResponseDelegate<ListMatchResponse> callback)
		{
			Uri uri = new Uri(this.baseUri, "/json/reply/ListMatchRequest");
			Debug.Log("MatchMakingClient ListMatches :" + uri);
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("projectId", Application.cloudProjectId);
			wWWForm.AddField("sourceId", Utility.GetSourceID().ToString());
			wWWForm.AddField("appId", Utility.GetAppID().ToString());
			wWWForm.AddField("includePasswordMatches", req.includePasswordMatches.ToString());
			wWWForm.AddField("accessTokenString", 0);
			wWWForm.AddField("domain", 0);
			wWWForm.AddField("pageSize", req.pageSize);
			wWWForm.AddField("pageNum", req.pageNum);
			wWWForm.AddField("nameFilter", req.nameFilter);
			wWWForm.headers["Accept"] = "application/json";
			WWW client = new WWW(uri.ToString(), wWWForm);
			return base.StartCoroutine(this.ProcessMatchResponse<ListMatchResponse>(client, callback));
		}

		[DebuggerHidden]
		private IEnumerator ProcessMatchResponse<JSONRESPONSE>(WWW client, NetworkMatch.ResponseDelegate<JSONRESPONSE> callback) where JSONRESPONSE : Response, new()
		{
			NetworkMatch.<ProcessMatchResponse>c__Iterator0<JSONRESPONSE> <ProcessMatchResponse>c__Iterator = new NetworkMatch.<ProcessMatchResponse>c__Iterator0<JSONRESPONSE>();
			<ProcessMatchResponse>c__Iterator.client = client;
			<ProcessMatchResponse>c__Iterator.callback = callback;
			<ProcessMatchResponse>c__Iterator.<$>client = client;
			<ProcessMatchResponse>c__Iterator.<$>callback = callback;
			return <ProcessMatchResponse>c__Iterator;
		}
	}
}
