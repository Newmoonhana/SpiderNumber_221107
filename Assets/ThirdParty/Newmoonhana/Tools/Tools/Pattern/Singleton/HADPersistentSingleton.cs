using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Newmoonhana.Tools
{
    /// <summary>
    /// 지속적인 싱글톤
    /// </summary>
    public class HADPersistentSingleton<T> : MonoBehaviour where T : Component
    {
        [Header("Persistent Singleton")]
        [Tooltip("true일시, 이 싱글톤은 OnAwake 시 자동으로 부모 해제")]
        public bool AutomaticallyUnparentOnAwake = true;

        public static bool HasInstance => _instance != null;    //_instance를 가진 지 체크
        public static T Current => _instance;   //현재 _instance

        protected static T _instance;
        protected bool _enabled;

		/// <summary>
		/// 싱글톤 패턴
		/// </summary>
		/// <value>instance</value>
		public static T Instance
		{
			get
			{
				if (_instance == null)  //_instance 미할당
				{
					_instance = FindObjectOfType<T>();	//스크립트가 들어간 오브젝트 find
					if (_instance == null)	//find해서도 존재하지 않으면 새로 생성
					{
						GameObject obj = new GameObject();
						obj.name = typeof(T).Name + "_AutoCreated";
						_instance = obj.AddComponent<T>();
					}
				}
				return _instance;	//반환
			}
		}

		/// <summary>
		/// Awake 시, 이미 하이어라키에 동일한 스크립트를 지닌 오브젝트가 있다면 그것을 제거
		/// </summary>
		protected virtual void Awake()
		{
			if (!Application.isPlaying)	//플레이 모드가 아닐 시 중단
			{
				return;
			}

			if (AutomaticallyUnparentOnAwake)
			{
				this.transform.SetParent(null);	//부모 해제
			}

			if (_instance == null)  //_instance가 미존재 == 첫 생성 instance
			{
				//만약 첫 생성 instance라면 싱글톤으로 설정
				_instance = this as T;
				DontDestroyOnLoad(transform.gameObject);
				_enabled = true;
			}
			else
			{
				//싱글톤이 이미 존재하고 find했을 시, 씬에 있는 다른 레퍼런스를 파괴
				if (this != _instance)
				{
					Destroy(this.gameObject);
				}
			}
		}
	}
}

