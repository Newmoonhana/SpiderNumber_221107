using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Newmoonhana.Tools
{
	/// <summary>
	/// �̱��� ����
	/// </summary>
	public class HADSingleton<T> : MonoBehaviour where T : Component
	{
		protected static T _instance;
		public static bool HasInstance => _instance != null;    //_instance�� ���� �� üũ
		public static T TryGetInstance() => HasInstance ? _instance : null;    //_instance�� ������ _instance�� ��ȯ
		public static T Current => _instance;   //���� _instance

		/// <summary>
		/// �̱��� ����
		/// </summary>
		/// <value>instance.</value>
		public static T Instance
		{
			get
			{
				if (_instance == null)  //_instance ���Ҵ�
				{
					_instance = FindObjectOfType<T>();  //��ũ��Ʈ�� �� ������Ʈ find
					if (_instance == null)  //find�ؼ��� �������� ������ ���� ����
					{
						GameObject obj = new GameObject();
						obj.name = typeof(T).Name + "_AutoCreated";
						_instance = obj.AddComponent<T>();
					}
				}
				return _instance;   //��ȯ
			}
		}

		/// <summary>
		/// Awake ��, �������̵�� base.Awake()�� ȣ���Ͽ� ����
		/// </summary>
		protected virtual void Awake()
		{
			if (!Application.isPlaying) //�÷��� ��尡 �ƴ� �� �ߴ�
			{
				return;
			}

			_instance = this as T;
		}
	}
}
