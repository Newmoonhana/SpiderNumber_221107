using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Newmoonhana.Tools
{
	/// <summary>
	/// 싱글톤 패턴
	/// </summary>
	public class HADSingleton<T> : MonoBehaviour where T : Component
	{
		protected static T _instance;
		public static bool HasInstance => _instance != null;    //_instance를 가진 지 체크
		public static T TryGetInstance() => HasInstance ? _instance : null;    //_instance를 가지면 _instance를 반환
		public static T Current => _instance;   //현재 _instance

		/// <summary>
		/// 싱글톤 패턴
		/// </summary>
		/// <value>instance.</value>
		public static T Instance
		{
			get
			{
				if (_instance == null)  //_instance 미할당
				{
					_instance = FindObjectOfType<T>();  //스크립트가 들어간 오브젝트 find
					if (_instance == null)  //find해서도 존재하지 않으면 새로 생성
					{
						GameObject obj = new GameObject();
						obj.name = typeof(T).Name + "_AutoCreated";
						_instance = obj.AddComponent<T>();
					}
				}
				return _instance;   //반환
			}
		}

		/// <summary>
		/// Awake 시, 오버라이드로 base.Awake()를 호출하여 설정
		/// </summary>
		protected virtual void Awake()
		{
			if (!Application.isPlaying) //플레이 모드가 아닐 시 중단
			{
				return;
			}

			_instance = this as T;
		}
	}
}
