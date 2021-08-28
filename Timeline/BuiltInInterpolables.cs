using System.Collections.Generic;
using Studio;
using ToolBox.Extensions;
using UnityEngine;
using AIChara;
using System.Xml;

namespace Timeline
{
    public static class BuiltInInterpolables
    {
        //TODO GLOBAL
        // Other plugins compatible

        // dark theme
        // loop the whole thing (with curves and stuff)
        // VideoExport events on the timeline

        // DONE
        // check keyframes disappearing with HSPE on chara replace (Bones, Boobs, Colliders, DynamicBones).
        // icons to context menu
        // color change for multiple interpolables

        public static void Populate()
        {
            Global();
            EnabledDisabled();
            Animation();
            TranslateRotationScale();

            CharacterClothingStates();
            CharacterJuice();
            CharacterStateMisc();
            CharacterNeck();
            CharacterEyesMouthHands();

            Item();

            Light();

            //Timeline. AddInterpolableModel(new InterpolableModel(
            //        owner: Timeline._ownerId,
            //        id: "debug",
            //        parameter: null,
            //        name: "Print Debug",
            //        interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => UnityEngine.Debug.LogError(message: "Interpolating Before " + Time.frameCount + " " + factor),
            //        interpolateAfter: (oci, parameter, leftValue, rightValue, factor) => UnityEngine.Debug.LogError(message: "Interpolating After " + Time.frameCount + " " + factor),
            //        isCompatibleWithTarget: (oci) => true,
            //        getValue: (oci, parameter) => null,
            //        readParameterFromXml: null,
            //        writeParameterToXml: null,
            //        readValueFromXml: null,
            //        writeValueToXml: null,
            //        useOciInHash: false
            //));
        }

        private static void Global()
        {
            Studio.CameraControl.CameraData globalCameraData = (Studio.CameraControl.CameraData)Studio.Studio.Instance.cameraCtrl.GetPrivate(name: "cameraData");
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "cameraOPos",
                    parameter: null,
                    name: "Camera Origin Position",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => globalCameraData.pos = Vector3.LerpUnclamped((Vector3)leftValue, (Vector3)rightValue, factor),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => true,
                    getValue: (oci, parameter) => globalCameraData.pos,
                    readValueFromXml: (parameter, node) => node.ReadVector3("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Vector3)o),
                    useOciInHash: false
            ));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "cameraORot",
                    parameter: null,
                    name: "Camera Origin Rotation",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => globalCameraData.rotate = Quaternion.SlerpUnclamped((Quaternion)leftValue, (Quaternion)rightValue, factor).eulerAngles,
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => true,
                    getValue: (oci, parameter) => Quaternion.Euler(globalCameraData.rotate),
                    readValueFromXml: (parameter, node) => node.ReadQuaternion("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Quaternion)o),
                    useOciInHash: false
            ));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "cameraOZoom",
                    parameter: null,
                    name: "Camera Zoom",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => globalCameraData.distance = new Vector3(x: globalCameraData.distance.x, y: globalCameraData.distance.y, z: Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => true,
                    getValue: (oci, parameter) => globalCameraData.distance.z,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o),
                    useOciInHash: false
            ));

            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "cameraFOV",
                    parameter: null,
                    name: "Camera FOV",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => Studio.Studio.Instance.cameraCtrl.fieldOfView = Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => true,
                    getValue: (oci, parameter) => Studio.Studio.Instance.cameraCtrl.fieldOfView,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o),
                    useOciInHash: false
            ));

            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "cameraPos",
                    parameter: null,
                    name: "Camera Position",
                    interpolateBefore: null,
                    interpolateAfter: (oci, parameter, leftValue, rightValue, factor) => Studio.Studio.Instance.cameraCtrl.mainCmaera.transform.position = Vector3.LerpUnclamped((Vector3)leftValue, (Vector3)rightValue, factor),
                    isCompatibleWithTarget: (oci) => true,
                    getValue: (oci, parameter) => Studio.Studio.Instance.cameraCtrl.mainCmaera.transform.position,
                    readValueFromXml: (parameter, node) => node.ReadVector3("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Vector3)o),
                    useOciInHash: false
            ));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "cameraRot",
                    parameter: null,
                    name: "Camera Rotation",
                    interpolateBefore: null,
                    interpolateAfter: (oci, parameter, leftValue, rightValue, factor) => Studio.Studio.Instance.cameraCtrl.mainCmaera.transform.rotation = Quaternion.SlerpUnclamped((Quaternion)leftValue, (Quaternion)rightValue, factor),
                    isCompatibleWithTarget: (oci) => true,
                    getValue: (oci, parameter) => Studio.Studio.Instance.cameraCtrl.mainCmaera.transform.rotation,
                    readValueFromXml: (parameter, node) => node.ReadQuaternion("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Quaternion)o),
                    useOciInHash: false
            ));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "timeScale",
                    parameter: null,
                    name: "Time Scale",
                    interpolateBefore: null,
                    interpolateAfter: (oci, parameter, leftValue, rightValue, factor) => Time.timeScale = Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor),
                    isCompatibleWithTarget: (oci) => true,
                    getValue: (oci, parameter) => Time.timeScale,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o),
                    useOciInHash: false
            ));
        }

        private static void EnabledDisabled()
        {
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "objectEnabled",
                    parameter: null,
                    name: "Enabled",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                    {
                        bool value = (bool)leftValue;
                        if (oci.treeNodeObject.visible != value)
                            oci.treeNodeObject.visible = value;
                    },
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci != null && oci is OCILight == false,
                    getValue: (oci, parameter) => oci.treeNodeObject.visible,
                    readValueFromXml: (parameter, node) => node.ReadBool("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (bool)o)
            ));
        }

        private static void TranslateRotationScale()
        {
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "guideObjectPos",
                    name: "Selected GuideObject Pos",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((GuideObject)parameter).changeAmount.pos = Vector3.LerpUnclamped((Vector3)leftValue, (Vector3)rightValue, factor),
                    interpolateAfter: null,
                    isCompatibleWithTarget: oci => oci != null,
                    getValue: (oci, parameter) => ((GuideObject)parameter).changeAmount.pos,
                    readValueFromXml: (parameter, node) => node.ReadVector3("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Vector3)o),
                    getParameter: oci => GuideObjectManager.Instance.selectObject,
                    readParameterFromXml: (oci, node) =>
                    {
                        Transform t = oci.guideObject.transformTarget.Find(node.Attributes["guideObjectPath"].Value);
                        if (t == null)
                            return null;
                        GuideObject guideObject;
                        Timeline._self._allGuideObjects.TryGetValue(t, out guideObject);
                        return guideObject;
                    },
                    writeParameterToXml: (oci, writer, o) => writer.WriteAttributeString("guideObjectPath", ((GuideObject)o).transformTarget.GetPathFrom(oci.guideObject.transformTarget)),
                    checkIntegrity: (oci, parameter, leftValue, rightValue) => parameter != null,
                    getFinalName: (name, oci, parameter) => $"GO Position ({((GuideObject)parameter).transformTarget.name})"
            ));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "guideObjectRot",
                    name: "Selected GuideObject Rot",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((GuideObject)parameter).changeAmount.rot = Quaternion.SlerpUnclamped((Quaternion)leftValue, (Quaternion)rightValue, factor).eulerAngles,
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci != null,
                    getValue: (oci, parameter) => Quaternion.Euler(((GuideObject)parameter).changeAmount.rot),
                    readValueFromXml: (parameter, node) => node.ReadQuaternion("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Quaternion)o),
                    getParameter: oci => GuideObjectManager.Instance.selectObject,
                    readParameterFromXml: (oci, node) =>
                    {
                        Transform t = oci.guideObject.transformTarget.Find(node.Attributes["guideObjectPath"].Value);
                        if (t == null)
                            return null;
                        GuideObject guideObject;
                        Timeline._self._allGuideObjects.TryGetValue(t, out guideObject);
                        return guideObject;
                    },
                    writeParameterToXml: (oci, writer, o) => writer.WriteAttributeString("guideObjectPath", ((GuideObject)o).transformTarget.GetPathFrom(oci.guideObject.transformTarget)),
                    checkIntegrity: (oci, parameter, leftValue, rightValue) => parameter != null,
                    getFinalName: (name, oci, parameter) => $"GO Rotation ({((GuideObject)parameter).transformTarget.name})"
            ));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "guideObjectScale",
                    name: "Selected GuideObject Scl",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((GuideObject)parameter).changeAmount.scale = Vector3.LerpUnclamped((Vector3)leftValue, (Vector3)rightValue, factor),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci != null,
                    getValue: (oci, parameter) => ((GuideObject)parameter).changeAmount.scale,
                    readValueFromXml: (parameter, node) => node.ReadVector3("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Vector3)o),
                    getParameter: oci => GuideObjectManager.Instance.selectObject,
                    readParameterFromXml: (oci, node) =>
                    {
                        Transform t = oci.guideObject.transformTarget.Find(node.Attributes["guideObjectPath"].Value);
                        if (t == null)
                            return null;
                        GuideObject guideObject;
                        Timeline._self._allGuideObjects.TryGetValue(t, out guideObject);
                        return guideObject;
                    },
                    writeParameterToXml: (oci, writer, o) => writer.WriteAttributeString("guideObjectPath", ((GuideObject)o).transformTarget.GetPathFrom(oci.guideObject.transformTarget)),
                    checkIntegrity: (oci, parameter, leftValue, rightValue) => parameter != null,
                    getFinalName: (name, oci, parameter) => $"GO Scale ({((GuideObject)parameter).transformTarget.name})"
            ));
        }

        private static void Animation()
        {
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "charAnimation",
                    parameter: null,
                    name: "Animation",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                    {
                        OCIChar chara = (OCIChar)oci;
                        OICharInfo.AnimeInfo info = (OICharInfo.AnimeInfo)leftValue;
                        if (chara.oiCharInfo.animeInfo.category != info.category || chara.oiCharInfo.animeInfo.group != info.group || chara.oiCharInfo.animeInfo.no != info.no)
                            chara.LoadAnime(info.group, info.category, info.no);
                    },
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIChar,
                    getValue: (oci, parameter) =>
                    {
                        OICharInfo.AnimeInfo info = ((OCIChar)oci).oiCharInfo.animeInfo;
                        return new OICharInfo.AnimeInfo() { category = info.category, group = info.group, no = info.no };
                    },
                    readValueFromXml: (parameter, node) => new OICharInfo.AnimeInfo() { category = node.ReadInt("valueCategory"), group = node.ReadInt("valueGroup"), no = node.ReadInt("valueNo") },
                    writeValueToXml: (parameter, writer, o) =>
                    {
                        OICharInfo.AnimeInfo info = (OICharInfo.AnimeInfo)o;
                        writer.WriteValue("valueCategory", info.category);
                        writer.WriteValue("valueGroup", info.group);
                        writer.WriteValue("valueNo", info.no);
                    }
            ));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "objectAnimationSpeed",
                    parameter: null,
                    name: "Animation Speed",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => oci.animeSpeed = Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci != null,
                    getValue: (oci, parameter) => oci.animeSpeed,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o)
            ));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "itemAnimationTime",
                    parameter: null,
                    name: "Animation Time",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                    {
                        Animator animator = ((OCIItem)oci).animator;
                        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
                        animator.Play(info.shortNameHash, 0, Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor));
                    },
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) =>
                    {
                        OCIItem item = oci as OCIItem;
                        return item != null && item.isAnime;
                    },
                    getValue: (oci, parameter) => ((OCIItem)oci).animator.GetCurrentAnimatorStateInfo(0).normalizedTime,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o)
            ));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "charAnimationTime",
                    parameter: null,
                    name: "Animation Time",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                    {
                        Animator animator = ((OCIChar)oci).charAnimeCtrl.animator;
                        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
                        animator.Play(info.shortNameHash, 0, Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor));
                        //animator.Update(0f);
                    },
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIChar,
                    getValue: (oci, parameter) => ((OCIChar)oci).charAnimeCtrl.animator.GetCurrentAnimatorStateInfo(0).normalizedTime,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o)
            ));
        }

        private static void CharacterClothingStates()
        {
            Dictionary<ChaFileDefine.ClothesKind, string> femaleClothes = new Dictionary<ChaFileDefine.ClothesKind, string>()
            {
                {ChaFileDefine.ClothesKind.top, "Top"},
                {ChaFileDefine.ClothesKind.bot, "Bottom"},
                {ChaFileDefine.ClothesKind.inner_t, "Inner Top"},
                {ChaFileDefine.ClothesKind.inner_b, "Inner Bottom"},
                /*
                {ChaFileDefine.ClothesKind.bra, "Bra"},
                {ChaFileDefine.ClothesKind.shorts, "Panties"},
                {ChaFileDefine.ClothesKind.swimsuitTop, "Upper Swimsuit"},
                {ChaFileDefine.ClothesKind.swimsuitBot, "Lower Swimsuit"},
                {ChaFileDefine.ClothesKind.swimClothesTop, "Swim Top"},
                {ChaFileDefine.ClothesKind.swimClothesBot, "Swim Bottom"},
                */
                {ChaFileDefine.ClothesKind.gloves, "Gloves"},
                {ChaFileDefine.ClothesKind.panst, "Pantyhose"},
                {ChaFileDefine.ClothesKind.socks, "Socks"},
                {ChaFileDefine.ClothesKind.shoes, "Shoes"},
            };

            foreach (KeyValuePair<ChaFileDefine.ClothesKind, string> pair in femaleClothes)
            {
                Timeline.AddInterpolableModel(new InterpolableModel(
                        owner: Timeline._ownerId,
                        id: "femaleClothes",
                        parameter: (int)pair.Key,
                        name: $"{pair.Value} State",
                        interpolateBefore: InterpolateClothes,
                        interpolateAfter: null,
                        isCompatibleWithTarget: (oci) => oci is OCICharFemale,
                        getValue: GetClothesValue,
                        readValueFromXml: (parameter, node) => node.ReadByte("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (byte)o),
                        readParameterFromXml: (oci, node) => node.ReadInt("parameter"),
                        writeParameterToXml: (oci, writer, o) => writer.WriteValue("parameter", (int)o),
                        getFinalName: (n, oci, parameter) => $"{femaleClothes[(ChaFileDefine.ClothesKind)(int)parameter]} State"
                        ));
            }

            Dictionary<ChaFileDefine.ClothesKind, string> maleClothes = new Dictionary<ChaFileDefine.ClothesKind, string>()
            {
                {ChaFileDefine.ClothesKind.top, "Clothes"},
                {ChaFileDefine.ClothesKind.shoes, "Shoes"},
            };

            foreach (KeyValuePair<ChaFileDefine.ClothesKind, string> pair in maleClothes)
            {
                Timeline.AddInterpolableModel(new InterpolableModel(
                        owner: Timeline._ownerId,
                        id: "maleClothes",
                        parameter: (int)pair.Key,
                        name: $"{pair.Value} State",
                        interpolateBefore: InterpolateClothes,
                        interpolateAfter: null,
                        isCompatibleWithTarget: (oci) => oci is OCICharMale,
                        getValue: GetClothesValue,
                        readValueFromXml: (parameter, node) => node.ReadByte("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (byte)o),
                        readParameterFromXml: (oci, node) => node.ReadInt("parameter"),
                        writeParameterToXml: (oci, writer, o) => writer.WriteValue("parameter", (int)o),
                        getFinalName: (n, oci, parameter) => $"{maleClothes[(ChaFileDefine.ClothesKind)(int)parameter]} State"

                ));
            }
        }

        private static void InterpolateClothes(ObjectCtrlInfo oci, object parameter, object leftValue, object rightValue, float factor)
        {
            int index = (int)parameter;
            byte value = (byte)leftValue;
            if ((byte)GetClothesValue(oci, parameter) != value)
                ((OCIChar)oci).SetClothesState(index, value);
        }

        private static object GetClothesValue(ObjectCtrlInfo oci, object parameter)
        {
            return ((OCIChar)oci).charFileStatus.clothesState[(int)parameter];
        }

        private static void CharacterJuice()
        {
            Dictionary<ChaFileDefine.SiruParts, string> juice = new Dictionary<ChaFileDefine.SiruParts, string>()
            {
                /*
                {ChaFileDefine.SiruParts.top, "Top"},
                {ChaFileDefine.SiruParts.bot, "Bottom"},
                {ChaFileDefine.SiruParts.bra, "Bra"},
                {ChaFileDefine.SiruParts.shorts, "Panties"},
                {ChaFileDefine.SiruParts.swim, "Swimsuit"},

                */
            };

            foreach (KeyValuePair<ChaFileDefine.SiruParts, string> pair in juice)
            {
                Timeline.AddInterpolableModel(new InterpolableModel(
                        owner: Timeline._ownerId,
                        id: "juice",
                        parameter: (int)pair.Key,
                        name: $"{pair.Value} Juice State",
                        interpolateBefore: InterpolateJuice,
                        interpolateAfter: null,
                        isCompatibleWithTarget: (oci) => oci is OCICharFemale,
                        getValue: GetJuiceValue,
                        readValueFromXml: (parameter, node) => node.ReadByte("value"),
                        writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (byte)o),
                        readParameterFromXml: (oci, node) => node.ReadInt("parameter"),
                        writeParameterToXml: (oci, writer, o) => writer.WriteValue("parameter", (int)o)
                ));
            }
        }

        private static void InterpolateJuice(ObjectCtrlInfo oci, object parameter, object leftValue, object rightValue, float factor)
        {
            ChaFileDefine.SiruParts index = (ChaFileDefine.SiruParts)(int)parameter;
            byte value = (byte)leftValue;
            if ((byte)GetJuiceValue(oci, parameter) != value)
                ((OCIChar)oci).SetSiruFlags(index, value);
        }

        private static object GetJuiceValue(ObjectCtrlInfo oci, object parameter)
        {
            return ((OCIChar)oci).GetSiruFlags((ChaFileDefine.SiruParts)(int)parameter);
        }

        private static void CharacterStateMisc()
        {
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "femaleTears",
                    parameter: null,
                    name: "Tears",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                    {
                        byte value = (byte)leftValue;
                        if (((OCIChar)oci).GetTears() != value)
                            ((OCIChar)oci).SetTears(_state: value);
                    },
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCICharFemale,
                    getValue: (oci, parameter) => ((OCIChar)oci).GetTears(),
                    readValueFromXml: (parameter, node) => node.ReadByte("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (byte)o)));

            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "femaleBlush",
                    parameter: null,
                    name: "Blush",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCIChar)oci).SetHohoAkaRate(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCICharFemale,
                    getValue: (oci, parameter) => ((OCIChar)oci).GetHohoAkaRate(),
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o)));

            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "femaleNipples",
                    parameter: null,
                    name: "Nipples",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCIChar)oci).SetNipStand(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCICharFemale,
                    getValue: (oci, parameter) => ((OCIChar)oci).oiCharInfo.nipple,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o)));

            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "femaleSkinShine",
                    parameter: null,
                    name: "Skin Shine",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCIChar)oci).SetTuyaRate(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCICharFemale,
                    getValue: (oci, parameter) => ((OCIChar)oci).oiCharInfo.SkinTuyaRate,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o)));

        }

        private static void CharacterNeck()
        {
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "characterNeck",
                    parameter: null,
                    name: "Neck Direction",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                    {
                        int value = (int)leftValue;
                        if (((OCIChar)oci).charFileStatus.neckLookPtn != value)
                            ((OCIChar)oci).ChangeLookNeckPtn(value);
                    },
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIChar,
                    getValue: (oci, parameter) => ((OCIChar)oci).charFileStatus.neckLookPtn,
                    readValueFromXml: (parameter, node) => node.ReadInt("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (int)o)));
        }

        private static void CharacterEyesMouthHands()
        {
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "characterEyes",
                    parameter: null,
                    name: "Eyes",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                    {
                        int value = (int)leftValue;
                        if (((OCIChar)oci).charInfo.GetEyesPtn() != value)
                            ((OCIChar)oci).charInfo.ChangeEyesPtn(value, false);
                    },
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIChar,
                    getValue: (oci, parameter) => ((OCIChar)oci).charInfo.GetEyesPtn(),
                    readValueFromXml: (parameter, node) => node.ReadInt("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (int)o)));

            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "characterEyesOpen",
                    parameter: null,
                    name: "Eyes Open",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCIChar)oci).charInfo.ChangeEyesOpenMax(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIChar,
                    getValue: (oci, parameter) => ((OCIChar)oci).charInfo.GetEyesOpenMax(),
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o)));

            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "characterMouth",
                    parameter: null,
                    name: "Mouth",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                    {
                        int value = (int)leftValue;
                        if (((OCIChar)oci).charInfo.GetMouthPtn() != value)
                            ((OCIChar)oci).charInfo.ChangeMouthPtn(value, false);
                    },
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIChar,
                    getValue: (oci, parameter) => ((OCIChar)oci).charInfo.GetMouthPtn(),
                    readValueFromXml: (parameter, node) => node.ReadInt("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (int)o)));

            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "characterMouthOpen",
                    parameter: null,
                    name: "Mouth Open",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCIChar)oci).ChangeMouthOpen(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIChar,
                    getValue: (oci, parameter) => ((OCIChar)oci).oiCharInfo.mouthOpen,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o)));

            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "characterLeftHand",
                    parameter: null,
                    name: "Left Hand",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                    {
                        int value = (int)leftValue;
                        if (((OCIChar)oci).oiCharInfo.handPtn[0] != value)
                            ((OCIChar)oci).ChangeHandAnime(0, value);
                    },
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIChar,
                    getValue: (oci, parameter) => ((OCIChar)oci).oiCharInfo.handPtn[0],
                    readValueFromXml: (parameter, node) => node.ReadInt("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (int)o)));

            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "characterRightHand",
                    parameter: null,
                    name: "Right Hand",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                    {
                        int value = (int)leftValue;
                        if (((OCIChar)oci).oiCharInfo.handPtn[1] != value)
                            ((OCIChar)oci).ChangeHandAnime(1, value);
                    },
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIChar,
                    getValue: (oci, parameter) => ((OCIChar)oci).oiCharInfo.handPtn[1],
                    readValueFromXml: (parameter, node) => node.ReadInt("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (int)o)));
        }

        private static void Item()
        {
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "itemColor",
                    parameter: null,
                    name: "Color",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCIItem)oci).SetColor(Color.LerpUnclamped((Color)leftValue, (Color)rightValue, factor), 0),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIItem,
                    getValue: (oci, parameter) => ((OCIItem)oci).itemInfo.colors[0].mainColor,
                    readValueFromXml: (parameter, node) => node.ReadColor("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Color)o)));
            /*
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "itemSpecColor",
                    parameter: null,
                    name: "Specular Color",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCIItem)oci).SetGlossiness(0, Color.LerpUnclamped((Color)leftValue, (Color)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIItem,
                    getValue: (oci, parameter) => ((OCIItem)oci).itemInfo.color.rgbSpecular,
                    readValueFromXml: (parameter, node) => node.ReadColor("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Color)o)));
            */
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "itemMetallic",
                    parameter: null,
                    name: "Metallic",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCIItem)oci).SetMetallic(0, Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIItem,
                    getValue: (oci, parameter) => ((OCIItem)oci).itemInfo.colors[0].metallic,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o)));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "itemSmoothness",
                    parameter: null,
                    name: "Smoothness",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCIItem)oci).SetGlossiness(0, Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIItem,
                    getValue: (oci, parameter) => ((OCIItem)oci).itemInfo.colors[0].glossiness,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o)));
            /*
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "itemColor2",
                    parameter: null,
                    name: "Color 2",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCIItem)oci).SetColor(Color.LerpUnclamped((Color)leftValue, (Color)rightValue, factor), 1),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIItem && ((OCIItem)oci).isColor2,
                    getValue: (oci, parameter) => ((OCIItem)oci).itemInfo.color2.rgbaDiffuse,
                    readValueFromXml: (parameter, node) => node.ReadColor("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Color)o)));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "itemSpecColor2",
                    parameter: null,
                    name: "Specular Color 2",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCIItem)oci).SetGlossiness(1, Color.LerpUnclamped((Color)leftValue, (Color)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIItem && ((OCIItem)oci).isColor2,
                    getValue: (oci, parameter) => ((OCIItem)oci).itemInfo.color2.rgbSpecular,
                    readValueFromXml: (parameter, node) => node.ReadColor("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Color)o)));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "itemSmoothness2",
                    parameter: null,
                    name: "Smoothness 2",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCIItem)oci).SetSharpness2(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCIItem && ((OCIItem)oci).isColor2,
                    getValue: (oci, parameter) => ((OCIItem)oci).itemInfo.color2.specularSharpness,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o)));
            */

        }
        private static void Light()
        {
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "lightColor",
                    parameter: null,
                    name: "Color",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCILight)oci).SetColor(Color.LerpUnclamped((Color)leftValue, (Color)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCILight,
                    getValue: (oci, parameter) => ((OCILight)oci).lightInfo.color,
                    readValueFromXml: (parameter, node) => node.ReadColor("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (Color)o)));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "lightOnOff",
                    parameter: null,
                    name: "On/Off",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                    {
                        bool value = (bool)leftValue;
                        if (((OCILight)oci).light.enabled != value)
                            ((OCILight)oci).SetEnable(value);
                    },
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCILight,
                    getValue: (oci, parameter) => ((OCILight)oci).light.enabled,
                    readValueFromXml: (parameter, node) => node.ReadBool("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (bool)o)));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "lightShadows",
                    parameter: null,
                    name: "Shadows",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) =>
                    {
                        bool value = (bool)leftValue;
                        if (((OCILight)oci).light.shadows != LightShadows.None != value)
                            ((OCILight)oci).SetShadow(value);
                    },
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCILight,
                    getValue: (oci, parameter) => ((OCILight)oci).light.shadows != LightShadows.None,
                    readValueFromXml: (parameter, node) => node.ReadBool("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (bool)o)));
            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "lightStrength",
                    parameter: null,
                    name: "Strength",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCILight)oci).SetIntensity(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCILight,
                    getValue: (oci, parameter) => ((OCILight)oci).light.intensity,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o)));

            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "lightRange",
                    parameter: null,
                    name: "Range",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCILight)oci).SetRange(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCILight && (((OCILight)oci).lightType == LightType.Point || ((OCILight)oci).lightType == LightType.Spot),
                    getValue: (oci, parameter) => ((OCILight)oci).light.range,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o)));

            Timeline.AddInterpolableModel(new InterpolableModel(
                    owner: Timeline._ownerId,
                    id: "lightSpotAngle",
                    parameter: null,
                    name: "Spot Angle",
                    interpolateBefore: (oci, parameter, leftValue, rightValue, factor) => ((OCILight)oci).SetSpotAngle(Mathf.LerpUnclamped((float)leftValue, (float)rightValue, factor)),
                    interpolateAfter: null,
                    isCompatibleWithTarget: (oci) => oci is OCILight && ((OCILight)oci).lightType == LightType.Spot,
                    getValue: (oci, parameter) => ((OCILight)oci).light.spotAngle,
                    readValueFromXml: (parameter, node) => node.ReadFloat("value"),
                    writeValueToXml: (parameter, writer, o) => writer.WriteValue("value", (float)o)));
        }
    }
}
