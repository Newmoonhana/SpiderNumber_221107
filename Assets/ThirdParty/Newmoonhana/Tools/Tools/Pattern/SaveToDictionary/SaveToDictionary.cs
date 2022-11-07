using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Newmoonhana.Tools
{
    //값이 new나 Resource.Load같이 여러번 호출되면 최적화에 좋지 않아 따로 저장해서 불러오는 구조가 필요할때 쓰는 부모 클래스

    public class SaveToDictionary_Data<TKey>   //저장할 데이터 클래스
    {
        public SaveToDictionary_Data() { }
        public virtual TKey Key { set { } }    //key를 세팅하면 value 값이 저장되도록(SaveToDictionary_Data의 자식에서 override로 구현)
    }

    public class SaveToDictionary<TKey, UValue> where UValue : SaveToDictionary_Data<TKey>, new()   //불러온 데이터 저장 & 저장된 데이터 불러오기
    {
        Dictionary<TKey, UValue> value_lst = new Dictionary<TKey, UValue>();
        public Dictionary<TKey, UValue> Value_lst { get { return value_lst; } }
        public int Count { get { return value_lst.Count; } }

        public virtual UValue GetValue(TKey _key)
        {
            AddValue(_key);
            return value_lst[_key];
        }

        bool AddValue(TKey _key)
        {
            if (value_lst.ContainsKey(_key))    //중복값
                return false;

            UValue _value = new UValue
            {
                Key = _key
            };
            value_lst.Add(_key, _value);   //Resource.Load로 불러올 때 최적화 기법
            //if (sortKeySelector != null)  //만약 추가될때마다 정렬한다면 이거지만 시작 시에 저장 용도로 추가하기만 해도 엄청 돌아 버리므로 일단 주석처리
            //    if (Count > 1)
            //        SortOrderBy();

            return true;
        }
        public void RemoveValue(TKey _key)
        {
            value_lst.Remove(_key);
        }
    }
}