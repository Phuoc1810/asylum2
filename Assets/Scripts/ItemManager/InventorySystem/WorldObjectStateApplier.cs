using UnityEngine;

[DisallowMultipleComponent]
public class WorldObjectStateApplier : MonoBehaviour
{
    [SerializeField] private Interactable interactable;
    private void Awake()
    {
        if(interactable==null) interactable=GetComponent<Interactable>();
        if(interactable == null) return;

        var id = interactable.WorldObjectId;
        if(!string.IsNullOrEmpty(id) && WorldStateService.Instance !=null &&WorldStateService.Instance.IsPicked(id))
        {
            gameObject.SetActive(false);
        }    
    }
}
