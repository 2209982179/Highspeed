const Activity = './icon/mdg/0.Activity.bmp';
const Control_flow = './icon/mdg/0.Control_flow.bmp';
/** 需求图工具箱 */
export const RequirementToolSet = [
    {
        Group: "SysML Requirements",
        GroupAlias: "SysML Requirements",
        items: [
            {
                id: 'UML4SysML::CRequirement',
                Object_Type: 'Requirement',
                Stereotype: "CRequirement",
                idSeed: 1,
                icon: Activity,
                title: 'Requirement',
                width: 160,
                height: 80,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: { rounded: 1, whiteSpace: 'wrap', html: 1, fillColor: '#dbf4d6', gradientColor: '#f1fbef', strokeColor: '#333333', gradientDirection: 'east' }
            },
            {
                id: 'UML4SysML::TestCase',
                Object_Type: 'Activity',
                Stereotype: "testCase",
                idSeed: 1,
                icon: Activity,
                title: 'Test Case',
                width: 180,
                height: 100,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: { rounded: 1, whiteSpace: 'wrap', html: 1, fillColor: '#f7f0bf', gradientColor: '#fbf7db', strokeColor: '#333333', gradientDirection: 'east' }
            },
        ]
    },
    {
        Group: "SysML Requirement Relationships",
        GroupAlias: "SysML Requirement Relationships",
        items:[
            {
                id: 'UML4SysML::Containment',
                Stereotype: "Nesting",
                Connector_Type: "Nesting",
                title: "Containment",
                idSeed: 1,
                icon: Control_flow,
                width: 1,
                isEdge: true,
                isPort: false,
                nodeType: 'link',
                dropAble: true, // 是否可以作为drop的对象
                style: { endArrow: 'circlePlus', html: 1, endFill: 0, rounded: 0, strokeColor: '#000000' }
            },
            {
                id: 'UML4SysML::Trace',
                Stereotype: "trace",
                Connector_Type: "Dependency",
                title: "Trace",
                idSeed: 1,
                icon: Control_flow,
                width: 1,
                isEdge: true,
                isPort: false,
                nodeType: 'link',
                dropAble: true, // 是否可以作为drop的对象
                style: { endArrow: 'openThin', html: 1, rounded: 0, endFill: 0, strokeWidth: 1, endSize: 12, dashed: 1, jumpSize: 6, strokeColor: '#000000' }
            },
            {
                id: 'UML4SysML::Copy',
                Stereotype: "copy",
                Connector_Type: "Dependency",
                title: "Copy",
                idSeed: 1,
                icon: Control_flow,
                width: 1,
                isEdge: true,
                isPort: false,
                nodeType: 'link',
                dropAble: true, // 是否可以作为drop的对象
                style: { endArrow: 'openThin', html: 1, rounded: 0, endFill: 0, strokeWidth: 1, endSize: 12, dashed: 1, jumpSize: 6, strokeColor: '#000000' }
            },
            {
                id: 'UML4SysML::Derive',
                Stereotype: "deriveReqt",
                Connector_Type: "Dependency",
                title: "Derive",
                idSeed: 1,
                icon: Control_flow,
                width: 1,
                isEdge: true,
                isPort: false,
                nodeType: 'link',
                dropAble: true, // 是否可以作为drop的对象
                style: { endArrow: 'openThin', html: 1, rounded: 0, endFill: 0, strokeWidth: 1, endSize: 12, dashed: 1, jumpSize: 6, strokeColor: '#000000' }
            },
            {
                id: 'UML4SysML::Verify',
                Stereotype: "verify",
                Connector_Type: "Dependency",
                title: "Verify",
                idSeed: 1,
                icon: Control_flow,
                width: 1,
                isEdge: true,
                isPort: false,
                nodeType: 'link',
                dropAble: true, // 是否可以作为drop的对象
                style: { endArrow: 'openThin', html: 1, rounded: 0, endFill: 0, strokeWidth: 1, endSize: 12, dashed: 1, jumpSize: 6, strokeColor: '#000000' }
            },
            {
                id: 'UML4SysML::Refine',
                Stereotype: "refine",
                Connector_Type: "Dependency",
                title: "Refine",
                idSeed: 1,
                icon: Control_flow,
                width: 1,
                isEdge: true,
                isPort: false,
                nodeType: 'link',
                dropAble: true, // 是否可以作为drop的对象
                style: { endArrow: 'openThin', html: 1, rounded: 0, endFill: 0, strokeWidth: 1, endSize: 12, dashed: 1, jumpSize: 6, strokeColor: '#000000' }
            },
            {
                id: 'UML4SysML::Satisfy',
                Stereotype: "satisfy",
                Connector_Type: "Dependency",
                title: "Satisfy",
                idSeed: 1,
                icon: Control_flow,
                width: 1,
                isEdge: true,
                isPort: false,
                nodeType: 'link',
                dropAble: true, // 是否可以作为drop的对象
                style: { endArrow: 'openThin', html: 1, rounded: 0, endFill: 0, strokeWidth: 1, endSize: 12, dashed: 1, jumpSize: 6, strokeColor: '#000000' }
            },
        ]
    }
]