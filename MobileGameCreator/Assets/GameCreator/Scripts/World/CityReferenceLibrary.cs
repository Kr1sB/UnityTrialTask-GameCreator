using UnityEngine;


[CreateAssetMenu(
    fileName = "New City Reference Library",
    menuName = "GameCreator/City Reference Library"
)]
public class CityReferenceLibrary : ScriptableObject
{
    public CityReference[] cities;

    public CityReference GetByIndex(int index)
    {
        if (index < 0 || index >= cities.Length)
            return null;

        return cities[index];
    }

    public CityReference GetById(uint id)
    {
        for(int i=0; i < cities.Length; ++i)
        {
            if (cities[i].id == id)
                return cities[i];
        }

        return null;
    }
}
