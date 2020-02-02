﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;


public class PlantFactory : MonoBehaviour
{
    private ObjectPool myPool;

    public Transform spawnOrientation;

    [System.Serializable]
    public class PlantBottom
    {
        public string tag;

        //Only a single bottom is needed as the soil prefab must match the pot prefab

        public GameObject potNormal;
        public GameObject potBroken;

        public GameObject soilNormal;
        public GameObject soilDry;
        public GameObject soilNeedsFertalizer;
    }

    [System.Serializable]
    public class PlantStem
    {
        public string tag;

        public GameObject stemNormal;
        public GameObject stemLimp;
        public GameObject stemBroken;
    }

    [System.Serializable]
    public class PlantHead
    {
        public string tag;

        public GameObject headNormal;
        public GameObject headPetalMissing;
    }


    public List<PlantBottom> plantBottoms;
    public List<PlantStem> plantStems;
    public List<PlantHead> plantHeads;

    public enum PoolObjects
    {
        pot_normal,
        pot_broken,

        soil_normal,
        soil_dry,
        soil_needs_fertilizer,

        stem_normal,
        stem_limp,
        stem_broken,

        head_normal,
        head_petal_missing
    }

    public Dictionary<PoolObjects, List<string>> PoolEnumListStrLookup = new Dictionary<PoolObjects, List<string>>();


    // Start is called before the first frame update
    void Start()
    {
        myPool = ObjectPool.Instance;

        int instances = 20;

        foreach (PlantBottom pb in plantBottoms) {
            addToStructures(pb.tag, "_pot_normal", PoolObjects.pot_normal, pb.potNormal, instances);
            addToStructures(pb.tag, "_pot_broken", PoolObjects.pot_broken, pb.potBroken, instances);

            addToStructures(pb.tag, "_soil_normal", PoolObjects.soil_normal, pb.soilNormal, instances);
            addToStructures(pb.tag, "_soil_dry", PoolObjects.soil_dry, pb.soilDry, instances);
            addToStructures(pb.tag, "_soil_needs_fertilizer", PoolObjects.soil_needs_fertilizer, pb.soilNeedsFertalizer, instances);

            Debug.Log("MyPool: " + myPool.poolDict);
        }

        foreach (PlantStem ps in plantStems)
        {
            addToStructures(ps.tag, "_stem_normal", PoolObjects.stem_normal, ps.stemNormal, instances);
            addToStructures(ps.tag, "_stem_limp", PoolObjects.stem_limp, ps.stemLimp, instances);
            addToStructures(ps.tag, "_stem_broken", PoolObjects.stem_broken, ps.stemBroken, instances);
        }

        foreach (PlantHead ph in plantHeads)
        {
            addToStructures(ph.tag, "_head_normal", PoolObjects.head_normal, ph.headNormal, instances);
            addToStructures(ph.tag, "_head_petal_missing", PoolObjects.head_petal_missing, ph.headPetalMissing, instances);
        }
    }

    public bool CreateFlower;
    private void Update()
    {
        if (CreateFlower)
        {
            CreatePlant();
            CreateFlower = false;
        }
        
    }

    private void addToStructures(string tag_prefix, string tag_suffix, PoolObjects pool_enum_val, GameObject prefab, int pool_instances)
    {
        string tag = tag_prefix + tag_suffix;
        myPool.AddPool(new ObjectPool.Pool(tag, prefab, pool_instances));

        //Debug.Log(PoolEnumListStrLookup[pool_enum_val]);

        if (PoolEnumListStrLookup.ContainsKey(pool_enum_val))
        {
            PoolEnumListStrLookup[pool_enum_val].Add(tag);
        }
        else
        {
            PoolEnumListStrLookup.Add(pool_enum_val, new List<string>() { tag });
        }

    }

    public enum HeadState
    {
        normal,
        missing_petals,
    }
    public Dictionary<HeadState, PoolObjects> HeadStatePoolObjectMapping = new Dictionary<HeadState, PoolObjects>()
    {
            {HeadState.normal, PoolObjects.head_normal},
            {HeadState.missing_petals, PoolObjects.head_petal_missing},
    };
        

    public enum StemState
    {
        normal,
        limp,
        broken,
    }
    public Dictionary<StemState, PoolObjects> StemStatePoolObjectMapping = new Dictionary<StemState, PoolObjects>()
    {
            {StemState.normal, PoolObjects.stem_normal},
            {StemState.limp, PoolObjects.stem_limp},
            {StemState.broken, PoolObjects.stem_broken},
    };
    public enum PotState
    {
        normal,
        broken,
    }
    public Dictionary<PotState, PoolObjects> PotStatePoolObjectMapping = new Dictionary<PotState, PoolObjects>()
    {
            {PotState.normal, PoolObjects.pot_normal},
            {PotState.broken, PoolObjects.pot_broken},
    };

    public enum SoilState
    {
        normal,
        dry,
        discolored,
    }
    public Dictionary<SoilState, PoolObjects> SoilStatePoolObjectMapping = new Dictionary<SoilState, PoolObjects>()
    {
            {SoilState.normal, PoolObjects.soil_normal},
            {SoilState.dry, PoolObjects.soil_dry},
            {SoilState.discolored, PoolObjects.soil_needs_fertilizer},
    };

    public enum PlantProblem
    {
        thirsty,
        broken_pot,
        bumped,
        plant_too_large,
        malnourished,
        infestation,
        sick,
        romantic_owner,
    }



    static T RandomEnumValue<T>()
    {
        var v = Enum.GetValues(typeof(T));
        return (T)v.GetValue(new System.Random().Next(v.Length));
    }

    static T RandomListElement<T>(List<T> list)
    {
        int index = (new System.Random()).Next(list.Count);
        return list[index];
    }

    private PlantProblem GenerateProblem()
    {
        return RandomEnumValue<PlantProblem>();
    }


    private Tuple<HeadState, StemState, PotState, SoilState> AttributeBuilder(PlantProblem problem)
    {
        //Tuple<HeadState, StemState, PotState, SoilState> plant_state;

        switch (problem){
            case PlantProblem.thirsty:
                return new Tuple<HeadState, StemState, PotState, SoilState>
                    (HeadState.normal, StemState.normal, PotState.normal, SoilState.normal);
            
            case PlantProblem.broken_pot:
                return new Tuple<HeadState, StemState, PotState, SoilState>
                    (HeadState.normal, StemState.normal, PotState.broken, SoilState.normal);

            case PlantProblem.bumped:
                return new Tuple<HeadState, StemState, PotState, SoilState>
                    (HeadState.normal, StemState.normal, PotState.broken, SoilState.normal);

            case PlantProblem.plant_too_large:
                return new Tuple<HeadState, StemState, PotState, SoilState>
                    (HeadState.normal, StemState.normal, PotState.broken, SoilState.normal);

            case PlantProblem.malnourished:
                return new Tuple<HeadState, StemState, PotState, SoilState>
                    (HeadState.normal, StemState.normal, PotState.broken, SoilState.normal);

            case PlantProblem.infestation:
                return new Tuple<HeadState, StemState, PotState, SoilState>
                    (HeadState.normal, StemState.normal, PotState.broken, SoilState.normal);

            case PlantProblem.sick:
                return new Tuple<HeadState, StemState, PotState, SoilState>
                    (HeadState.normal, StemState.normal, PotState.broken, SoilState.normal);

            case PlantProblem.romantic_owner:
                return new Tuple<HeadState, StemState, PotState, SoilState>
                    (HeadState.normal, StemState.normal, PotState.broken, SoilState.normal);

            default:
                return new Tuple<HeadState, StemState, PotState, SoilState>
                    (HeadState.normal, StemState.normal, PotState.normal, SoilState.normal);
        }

    }



    public GameObject CreatePlant()
    {
        PlantProblem problem = GenerateProblem();
        Tuple<HeadState, StemState, PotState, SoilState> attributes = AttributeBuilder(problem);


        //Pull data from the generated tuple
        HeadState ehead = attributes.Item1;
        StemState estem = attributes.Item2;
        PotState epot = attributes.Item3;
        SoilState esoil = attributes.Item4;



        string head_tag = RandomListElement<string>(PoolEnumListStrLookup[HeadStatePoolObjectMapping[ehead]]);
        string stem_tag = RandomListElement<string>(PoolEnumListStrLookup[StemStatePoolObjectMapping[estem]]);
        string pot_tag = RandomListElement<string>(PoolEnumListStrLookup[PotStatePoolObjectMapping[epot]]);
        string soil_tag = RandomListElement<string>(PoolEnumListStrLookup[SoilStatePoolObjectMapping[esoil]]);

        GameObject plant_parent = new GameObject();
        plant_parent.name = "PlantParent";
        plant_parent.transform.position = spawnOrientation.position;
        Debug.Log(plant_parent.transform.position);
        plant_parent.transform.rotation = spawnOrientation.rotation;
        plant_parent.AddComponent<Rigidbody>();

        GameObject plant_top = new GameObject();
        plant_top.name = "PlantTop";
        plant_top.transform.parent = plant_parent.transform;

        GameObject plant_stem = myPool.SpawnObject(stem_tag, new Vector3(0,0,0), new Quaternion(), plant_top.transform);
        GameObject plant_head = myPool.SpawnObject(head_tag, new Vector3(0, 0, 0), new Quaternion(), plant_stem.transform);
        GameObject plant_soil = myPool.SpawnObject(soil_tag, new Vector3(0, 0, 0), new Quaternion(), plant_top.transform);


        GameObject plant_bottom = new GameObject();
        plant_bottom.name = "PlantBottom";
        plant_bottom.transform.parent = plant_parent.transform;

        GameObject plant_pot = myPool.SpawnObject(pot_tag, new Vector3(0, 0, 0), new Quaternion(), plant_bottom.transform);

        return plant_parent;
    }


}

