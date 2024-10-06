using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class Tile : MonoBehaviour
{
    public TileState state {  get; private set; }
    public TileCell cell { get; private set; }  
    public int number {  get; private set; }

    private Image background;

    private TextMeshProUGUI text;

    public bool locked {  get; set; }
    private void Awake()
    {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetState(TileState state,int number)
    {
        this.state = state;

        this.number = number;

        background.color = state.backgroundColor;

        text.color = state.textColor;

        text.text = number.ToString();

       // Debug.Log($"SetState: {number}, Background Color: {state.backgroundColor}, Text Color: {state.textColor}");

    }
    // spawn a cell according to the cell get in
    // because the tile will be generated by the cell 
    // which is owned it 
    public void Spawn(TileCell cell)
    {
        if(this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;

        // key
        // set current tile as the cell which pass in 
        this.cell.tile = this;

        transform.position = cell.transform.position;
    }
    // Move to the position 
    public void MoveTo(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;

        this.cell.tile = this;

        StartCoroutine(Animate(cell.transform.position, false));
    }
    public void Merge(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }
        this.cell = null;
        cell.tile.locked = true;

        StartCoroutine(Animate(cell.transform.position,true));
    }

    // to implement position animation 
    // Vector3 to is the target position where tile wants to go 
    // the merging controls if it can be destroied
    private IEnumerator Animate(Vector3 to,bool merging)
    {
        float elapsed = 0f;
        float duration = 0.1f;
        Vector3 from = transform.position;
        while (elapsed < duration) { 
            transform.position = Vector3.Lerp(from,to,elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;

        if (merging) { 
            Destroy(gameObject);
        }

    }

}
