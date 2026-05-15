const Activity = './icon/mdg/0.Activity.bmp'
const Control_flow = './icon/mdg/0.Control_flow.bmp'


//顺序图工具箱
export const SeuenceToolSet = [
    {
        Group: "SysML Interactions",
        GroupAlias: "SysML Interactions",
        items: [
            {
                id: 'UML4SysML::Sequence',
                Object_Type: 'Sequence',
                Stereotype: "Sequence",
                idSeed: 1,
                icon: Activity,
                title: 'Sequence',
                width: 160,
                height: 600,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    shape: 'umlLifeline', perimeter: 'lifelinePerimeter', whiteSpace: 'wrap', html: 1, container: 1, dropTarget: 0, collapsible: 0, recursiveResize: 0, outlineConnect: 0, portConstraint: 'eastwest', newEdgeStyle: { "curved": 0, "rounded": 0 }, rounded: 1, fillColor: '#b2e7ea', strokeColor: '#000000'
                }
            },
            {
                id: 'UML4SysML::InteractionFragment',
                Object_Type: 'InteractionFragment',
                Stereotype: "InteractionFragment",
                idSeed: 1,
                icon: Activity,
                title: 'Fragment',
                width: 300,
                height: 200,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    shape: 'umlFrame', whiteSpace: 'wrap', html: 1, pointerEvents: 0, recursiveResize: 0, container: 1, collapsible: 0, width: 160, height: 19, align: 'left', fillColor: '#dae8fc', strokeColor: '#6c8ebf'
                }
            },
        ]
    },
    {
        Group: "SysML Interaction Relationships",
        GroupAlias: "SysML Interaction Relationships",
        items: [
            {
                id: 'UML4SysML::Message-Synchronous',
                title: "同步消息",
                Connector_Type: "Sequence",
                PDATA1 : 'Synchronous',
                PDATA2 : 'retval=void;',
                PDATA3 : 'Call',
                idSeed: 1,
                icon: Control_flow,
                width: 1,
                isEdge: true,
                isPort: false,
                nodeType: 'link',
                dropAble: true, // 是否可以作为drop的对象
                style: { endArrow: 'classic', html: 1, rounded: 0, endSize: 12, strokeColor: '#000000', fontColor: '#446299', verticalAlign: 'bottom', elbow: 'vertical' }
            },
            {
                id: 'UML4SysML::Message-Asynchronous',
                title: "异步消息",
                Connector_Type: "Sequence",
                PDATA1 : 'Asynchronous',
                PDATA2 : 'retval=void;',
                PDATA3 : 'Call',
                idSeed: 1,
                icon: Control_flow,
                width: 1,
                isEdge: true,
                isPort: false,
                nodeType: 'link',
                dropAble: true, // 是否可以作为drop的对象
                style: { endArrow: 'open', html: 1, rounded: 0, endSize: 12, strokeColor: '#000000', fontColor: '#446299', verticalAlign: 'bottom', elbow: 'vertical' }
            },
        ]
    }
]

