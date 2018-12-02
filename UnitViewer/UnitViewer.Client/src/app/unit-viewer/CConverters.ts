import * as PolygonGenerator from "@jci-ahu/ui.shared.polygon-generator";
import * as EnumsCommon from "@jci-ahu/data.common.enums";
import * as EnumsAHU from "@jci-ahu/data.ahu.enums";

export class CConverters
{
    public static get3DLocation(type: EnumsCommon.Common.E_UnitSide): PolygonGenerator.Enums.E_Location
    {
        if (type === EnumsCommon.Common.E_UnitSide.Front)
        {
            return PolygonGenerator.Enums.E_Location.Front;
        }
        else if (type === EnumsCommon.Common.E_UnitSide.Rear)
        {
            return PolygonGenerator.Enums.E_Location.Rear;
        }
        else if (type === EnumsCommon.Common.E_UnitSide.Bottom)
        {
            return PolygonGenerator.Enums.E_Location.Bottom;
        }
        else if (type === EnumsCommon.Common.E_UnitSide.Top)
        {
            return PolygonGenerator.Enums.E_Location.Top;
        }
        else if (type === EnumsCommon.Common.E_UnitSide.Left)
        {
            return PolygonGenerator.Enums.E_Location.Left;
        }
        else if (type === EnumsCommon.Common.E_UnitSide.Right)
        {
            return PolygonGenerator.Enums.E_Location.Right;
        }

        return PolygonGenerator.Enums.E_Location.Undefined;
    }
    public static get3DOpeningShape(type: EnumsAHU.Opening.E_OpeningShape): PolygonGenerator.Enums.E_OpeningShape
    {
        if (type === EnumsAHU.Opening.E_OpeningShape.Oblong)
        {
            return PolygonGenerator.Enums.E_OpeningShape.Oblong;
        }
        else if (type === EnumsAHU.Opening.E_OpeningShape.Rectangle)
        {
            return PolygonGenerator.Enums.E_OpeningShape.Rectangle;
        }
        else if (type === EnumsAHU.Opening.E_OpeningShape.Round)
        {
            return PolygonGenerator.Enums.E_OpeningShape.Round;
        }

        return PolygonGenerator.Enums.E_OpeningShape.Undefined;
    }
}