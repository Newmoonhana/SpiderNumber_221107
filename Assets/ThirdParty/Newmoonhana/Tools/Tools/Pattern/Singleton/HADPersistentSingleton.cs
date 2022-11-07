using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Newmoonhana.Tools
{
    /// <summary>
    /// �������� �̱���
    /// </summary>
    public class HADPersistentSingleton<T> : MonoBehaviour where T : Component
    {
        [Header("Persistent Singleton")]
        [Tooltip("true�Ͻ�, �� �̱����� OnAwake �� �ڵ����� �θ� ����")]
        public bool AutomaticallyUnparentOnAwake = true;

        public static bool HasInstance => _instance != null;    //_instance�� ���� �� üũ
        public static T Current => _instance;   //���� _instance

        protected static T _instance;
        protected bool _enabled;

		/// <summary>
		/// �̱��� ����
		/// </summary>
		/// <value>instance</value>
		public static T Instance
		{
			get
			{
				if (_instance == null)  //_instance ���Ҵ�
				{
					_instance = FindObjectOfType<T>();	//��ũ��Ʈ�� �� ������Ʈ find
					if (_instance == null)	//find�ؼ��� �������� ������ ���� ����
					{
						GameObject obj = new GameObject();
						obj.name = typeof(T).Name + "_AutoCreated";
						_instance = obj.AddComponent<T>();
					}
				}
				return _instance;	//��ȯ
			}
		}

		/// <summary>
		/// Awake ��, �̹� ���̾��Ű�� ������ ��ũ��Ʈ�� ���� ������Ʈ�� �ִٸ� �װ��� ����
		/// </summary>
		protected virtual void Awake()
		{
			if (!Application.isPlaying)	//�÷��� ��尡 �ƴ� �� �ߴ�
			{
				return;
			}

			if (AutomaticallyUnparentOnAwake)
			{
				this.transform.SetParent(null);	//�θ� ����
			}

			if (_instance == null)  //_instance�� ������ == ù ���� instance
			{
				//���� ù ���� instance��� �̱������� ����
				_instance = this as T;
				DontDestroyOnLoad(transform.gameObject);
				_enabled = true;
			}
			else
			{
				//�̱����� �̹� �����ϰ� find���� ��, ���� �ִ� �ٸ� ���۷����� �ı�
				if (this != _instance)
				{
					Destroy(this.gameObject);
				}
			}
		}
	}
}

