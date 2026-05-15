const Activity = './icon/mdg/0.Activity.bmp'
const Aciton_send_signal = './icon/mdg/0.Aciton_send_signal.bmp'
const Action = './icon/mdg/0.Action.bmp'
const Action_call_behavior = './icon/mdg/0.Action_call_behavior.bmp'
const Action_accept_event = './icon/mdg/0.Action_accept_event.bmp'
const Initial = './icon/mdg/0.Inital.bmp'
const Final = './icon/mdg/0.Final.bmp'
const Flow_final = './icon/mdg/0.Flow_final.bmp'
const Decision = './icon/mdg/0.Decision.bmp'
const Fork = './icon/mdg/0.Fork.bmp'
const Parameter = './icon/mdg/0.Parameter.bmp'
const Control_flow = './icon/mdg/0.Control_flow.bmp'
const Structured_activity = './icon/mdg/0.Structured_activity.bmp';
const Action_accept_event_timer = './icon/mdg/0.Action_accept_event_timer.bmp';
const Partition = './icon/mdg/0.Partition.bmp';
const Control_operator = './icon/mdg/0.Control_operator.bmp';
const Object_node = './icon/mdg/0.Object_node.bmp';
const Central_buffer_node = './icon/mdg/0.Central_buffer_node.bmp';
const Datastore = './icon/mdg/0.Datastore.bmp'
const Merge = './icon/mdg/0.Merge.bmp';
const Synch = './icon/mdg/0.Synch.bmp';
const Region = './icon/mdg/0.Region.bmp';
const Exception = './icon/mdg/0.Exception.bmp';
const Object_flow = './icon/mdg/0.Object_flow.bmp';
const Interrupt_flow = './icon/mdg/0.Interrupt_flow.bmp';


const ActionSYSML = {
    Action_CallBehavior: {
        id: 'UML4SysML::Action_CallBehavior',
        Object_Type: 'Action',
        Stereotype: "Action_CallBehavior",
        idSeed: 1,
        title: 'Call Behavior',
        icon: Action_call_behavior,
        width: 160,
        height: 80,
        nodeType: 'rectangle',
        dropAble: true, // 是否可以作为drop的对象
        NType: 0,
        style: {
            html: 1, shape: 'mxgraph.sysml.callBehAct', whiteSpace: 'wrap', align: 'center', verticalAlign: 'top',
            fillColor: '#facfa3', gradientDirection: 'east', gradientColor: '#fcdfc1', strokeColor: '#000000', fontColor: '#000000'
        },
        menuTitle: 'Call Behavior',
    },
    Action_AcceptEvent: {
        id: 'UML4SysML::Action_AcceptEvent',
        Object_Type: 'Action',
        Stereotype: 'Action_AcceptEvent',
        idSeed: 1,
        icon: Action_accept_event,
        title: 'Accept Event',
        width: 140,
        height: 60,
        nodeType: 'rectangle',
        dropAble: true, // 是否可以作为drop的对象
        menuTitle: 'Accept Event',
        style: {
            html: 1, shape: 'mxgraph.sysml.accEvent', strokeWidth: 1, whiteSpace: 'wrap', align: 'center',
            fillColor: '#facfa3', gradientDirection: 'east', gradientColor: '#fcdfc1', strokeColor: '#000000', fontColor: '#000000'
        }
    },
    Action_AcceptTimeEvent: {
        id: 'UML4SysML::Action_AcceptTimeEvent',
        Object_Type: 'Action',
        Stereotype: 'Action_AcceptTimeEvent',
        idSeed: 1,
        title: 'Accept Time Event',
        width: 40,
        icon: Action_accept_event_timer,
        height: 60,
        nodeType: 'rectangle',
        dropAble: true, // 是否可以作为drop的对象
        menuTitle: 'Accept Time Event',
        style: {
            shape: 'mxgraph.sysml.timeEvent', strokeWidth: 1, align: 'center', verticalAlign: 'top', horizontal: 1, labelPosition: 'center', verticalLabelPosition: 'bottom', html: 1, whiteSpace: 'wrap',
            fillColor: '#facfa3', gradientDirection: 'east', gradientColor: '#fcdfc1', strokeColor: '#000000', fontColor: '#000000'
        }
    },
    Action_SendSignal: {
        id: 'UML4SysML::Action_SendSignal',
        Object_Type: 'Action',
        Stereotype: 'Action_SendSignal',
        idSeed: 1,
        title: 'Send Signa',
        icon: Aciton_send_signal,
        width: 140,
        height: 60,
        nodeType: 'rectangle',
        dropAble: true, // 是否可以作为drop的对象
        menuTitle: 'Send Signal',
        style: {
            html: 1, shape: 'mxgraph.sysml.sendSigAct', strokeWidth: 1, whiteSpace: 'wrap', align: 'center',
            fillColor: '#facfa3', gradientDirection: 'east', gradientColor: '#fcdfc1', strokeColor: '#000000', fontColor: '#000000'
        }
    },
};

const ObjectNodeSysML = {
    id: 'UML4SysML::ActivityObject_ObjectNode',
    Object_Type: 'ObjectNode',
    Stereotype: null,
    idSeed: 1,
    icon: Parameter,
    title: 'ObjectNode',
    width: 15,
    height: 15,
    nodeType: 'rectangle',
    NType: 0,
    dropAble: true, // 是否可以作为drop的对象
    menuTitle: 'Edge Mounted',
    style: {
        html: 1, shape: 'mxgraph.sysml.port', html: 1, resizable: 0, sysMLPortType: 'empty', labelPosition: 'center', verticalLabelPosition: 'bottom',
        align: 'center', verticalAlign: 'top', fillColor: '#b1ddf0', strokeColor: '#10739e'
    },
    value: 'ObjectNode'
};

export const ActivityToolSet = [
    {
        Group: "SysML Activities",
        GroupAlias: "SysML Activities",
        items: [
            {
                id: 'UML4SysML::Activity',
                Object_Type: 'Activity',
                Stereotype: "Activity",
                idSeed: 1,
                icon: Activity,
                title: 'Activity',
                width: 160,
                height: 80,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    shape: 'rect',
                    html: 1,
                    rounded: 1,
                    whiteSpace: 'wrap',
                    align: 'center',
                    verticalAlign: 'top',
                    gradientColor: '#fbf7db', gradientDirection: 'east', fillColor: '#f7f0bf', strokeColor: '#000000', fontColor: '#000000'
                }
            },
            {
                id: 'UML4SysML::StructuredActivity',
                Object_Type: 'StructuredActivity',
                Stereotype: 'StructuredActivity',
                idSeed: 1,
                icon: Structured_activity,
                title: 'Structured Activity',
                width: 20,
                height: 20,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    shape: 'rect',
                    html: 1,
                    rounded: 1,
                    whiteSpace: 'wrap',
                    align: 'center',
                    fillColor: '#ffe6cc',
                    strokeColor: '#000000',
                },
                menuItem: [
                    {

                        id: 'UML4SysML::StructuredActivity_LoopNode',
                        Object_Type: 'LoopNode',
                        Stereotype: "StructuredActivity_LoopNode",
                        idSeed: 1,
                        title: 'Loop Node',
                        width: 200,
                        height: 180,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        NType: 17,
                        style: {
                            shape: 'rect', html: 1, overflow: 'fill', whiteSpace: 'wrap', dashed: 1, rounded: 1,
                            fillColor: '#ffe1bb', gradientDirection: 'east', gradientColor: '#ffefda', strokeColor: '#000000', fontColor: '#000000'
                        },
                        value: '<p style="margin:0px;margin-top:4px;text-align:center;">《loop》<br><b>LoopNode1</b></p><br><hr style="border-top: 1px dashed #000000;  border-bottom: none;" /><p style="margin:0px;margin-left:8px;text-align:left;">[Setup]</p><br><br><hr style="border-top: 1px dashed #000000;  border-bottom: none;" /><p style="margin:0px;margin-left:8px;text-align:left;">[Test]</p><br><br><hr style="border-top: 1px dashed #000000; border-bottom: none;" /><p style="margin:0px;margin-left:8px;text-align:left;">[Body]</p>',
                        menuTitle: 'Loop Node',
                    },
                    {

                        id: 'UML4SysML::StructuredActivity_ConditionalNode',
                        Object_Type: 'ConditionalNode',
                        Stereotype: "StructuredActivity_ConditionalNode",
                        idSeed: 1,
                        title: 'Conditional Node',
                        width: 200,
                        height: 180,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        NType: 18,
                        style: {
                            shape: 'rect', html: 1, overflow: 'fill', whiteSpace: 'wrap', dashed: 1, rounded: 1,
                            fillColor: '#ffe1bb', gradientDirection: 'east', gradientColor: '#ffefda', strokeColor: '#000000', fontColor: '#000000'
                        },
                        menuTitle: 'Conditional Node',
                        value: '<p style="margin:0px;margin-top:4px;text-align:center;">《conditional》<br><b>ConditionalNode1</b></p><br><hr style="border-top: 1px dashed #000000;  border-bottom: none;" /><p style="margin:0px;margin-left:8px;text-align:left;">[Test]</p><br><br><hr style="border-top: 1px dashed #000000;  border-bottom: none;" /><p style="margin:0px;margin-left:8px;text-align:left;">[Body]</p><br><br>'
                    },
                    {
                        id: 'UML4SysML::StructuredActivity_Other',
                        Object_Type: 'Activity',
                        Stereotype: 'StructuredActivity_Other',
                        idSeed: 1,
                        title: 'Activity_Other',
                        width: 160,
                        height: 80,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        menuTitle: 'Other',
                        style: {
                            html: 1, outlineConnect: 0, whiteSpace: 'wrap', shape: 'mxgraph.archimate3.application', appType: 'collab', archiType: 'rounded',
                            fillColor: '#ffe1bb', gradientDirection: 'east', gradientColor: '#ffefda', strokeColor: '#000000', fontColor: '#000000'
                        }
                    }
                ]
            },
            {
                id: 'UML4SysML::Action',
                idSeed: 1,
                icon: Action,
                title: 'Action',
                nodeType: 'rectangle',
                width: 20,
                height: 20,
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    fillColor: '#ffd5a9',
                },
                menuItem: [
                    {

                        id: 'UML4SysML::Action_Atomic',
                        Object_Type: 'Action',
                        Stereotype: "Action_Atomic",
                        idSeed: 1,
                        title: 'Atomic',
                        width: 160,
                        height: 80,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        style: {
                            shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center',
                            fillColor: '#facfa3', gradientDirection: 'east', gradientColor: '#fcdfc1', strokeColor: '#000000', fontColor: '#000000'
                        },
                        menuTitle: 'Atomic',
                    },
                    ActionSYSML.Action_CallBehavior,
                    {

                        id: 'UML4SysML::Action_CallOperation',
                        Object_Type: 'Action',
                        Stereotype: "Action_CallOperation",
                        idSeed: 1,
                        title: 'Call Operation',
                        width: 160,
                        height: 80,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        menuTitle: 'Call Operation',
                        style: {
                            shape: 'rect', html: 1, overflow: 'fill', whiteSpace: 'wrap', rounded: 1, verticalAlign: 'top',
                            fillColor: '#facfa3', gradientDirection: 'east', gradientColor: '#fcdfc1', strokeColor: '#000000', fontColor: '#000000'
                        },
                        value: `<p style="text-align:center;margin:0px;margin-top:4px;font-size:11px;"><b style="font-size:11px;">CallOperation1</b><br>(::)</p>`

                    },
                    ActionSYSML.Action_AcceptEvent,
                    ActionSYSML.Action_AcceptTimeEvent,

                    {
                        id: 'UML4SysML::Action_WriteVariable',
                        Object_Type: 'Action',
                        Stereotype: 'Action_WriteVariable',
                        idSeed: 1,
                        title: 'Write Variable',
                        width: 160,
                        height: 80,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        menuTitle: 'Write Variable',
                        style: {
                            shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center',
                            fillColor: '#facfa3', gradientDirection: 'east', gradientColor: '#fcdfc1', strokeColor: '#000000', fontColor: '#000000'
                        }
                    },
                    {
                        id: 'UML4SysML::Action_ValueSpecification',
                        Object_Type: 'Action',
                        Stereotype: 'Action_ValueSpecification',
                        idSeed: 1,
                        title: 'Value Specification',
                        width: 160,
                        height: 80,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        menuTitle: 'Value Specification',
                        style: {
                            shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center',
                            fillColor: '#facfa3', gradientDirection: 'east', gradientColor: '#fcdfc1', strokeColor: '#000000', fontColor: '#000000'
                        }
                    },
                    {
                        id: 'UML4SysML::Action_CreateObject',
                        Object_Type: 'Action',
                        Stereotype: 'Action_CreateObject',
                        idSeed: 1,
                        title: 'Create Object',
                        width: 160,
                        height: 80,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        menuTitle: 'Create Object',
                        NType: 0,
                        style: {
                            shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center',
                            fillColor: '#facfa3', gradientDirection: 'east', gradientColor: '#fcdfc1', strokeColor: '#000000', fontColor: '#000000'
                        }
                    },
                    {

                        id: 'UML4SysML::Action_DestroyObject',
                        Object_Type: 'Action',
                        Stereotype: 'Action_DestroyObject',
                        idSeed: 1,
                        title: 'Destroy Object',
                        width: 160,
                        height: 80,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        menuTitle: 'Destroy Object',
                        NType: 0,
                        style: {
                            shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center',
                            fillColor: '#facfa3', gradientDirection: 'east', gradientColor: '#fcdfc1', strokeColor: '#000000', fontColor: '#000000'
                        }

                    },
                    {

                        id: 'UML4SysML::Action_Other',
                        Object_Type: 'Action_Other',
                        Stereotype: 'Action_Other',
                        idSeed: 1,
                        title: 'Other',
                        width: 160,
                        height: 80,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        menuTitle: 'Other',
                        style: {
                            shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center', fillColor: '#ffd5a9', strokeColor: '#000000'
                        }

                    }
                ]
            },
            ActionSYSML.Action_CallBehavior,
            ActionSYSML.Action_AcceptEvent,
            ActionSYSML.Action_AcceptTimeEvent,
            ActionSYSML.Action_SendSignal,
            {
                id: 'UML4SysML::Partition',
                Object_Type: 'ActivityPartition',
                Stereotype: "ActivityPartition",
                idSeed: 1,
                icon: Partition,
                title: 'Partition',
                NType: 0,
                width: 300,
                height: 200,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    html: 1, dashed: 0, whiteSpace: 'wrap', shape: 'mxgraph.dfd.dataStoreID', align: 'center', spacingLeft: 3, points: [[0, 0], [0.5, 0], [1, 0], [0, 0.5], [1, 0.5], [0, 1], [0.5, 1], [1, 1]], horizontal: 0, fillColor: 'none', fontColor: '#000000', strokeColor: '#000000', direction: 'east', labelPosition: 'center', verticalLabelPosition: 'middle', verticalAlign: 'top', textDirection: 'ltr'
                }
            },
            {
                id: 'UML4SysML::Activity_ControlOperator',
                Object_Type: 'Activity_ControlOperator',
                Stereotype: "Activity_ControlOperator",
                idSeed: 1,
                icon: Control_operator,
                title: 'Control Operator...',
                width: 20,
                height: 20,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    fillColor: '#C2BEFE'
                },
                menuItem: [
                    {
                        id: 'UML4SysML::Activity_ControlOperator',
                        Object_Type: 'Activity',
                        Stereotype: 'Activity_ControlOperator',
                        NType: 0,
                        idSeed: 1,
                        title: 'Control Operator',
                        width: 160,
                        height: 80,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        menuTitle: 'Activity',
                        style: {
                            shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center', verticalAlign: 'top',
                            gradientColor: '#fbf7db', gradientDirection: 'east', fillColor: '#f7f0bf', strokeColor: '#000000', fontColor: '#000000'
                        }
                    },
                    {

                        id: 'UML4SysML::Activity_Interaction',
                        Object_Type: 'Interaction',
                        Stereotype: 'controlOperator',
                        idSeed: 1,
                        title: 'int',
                        width: 300,
                        height: 200,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        menuTitle: 'Interaction',
                        NType: 0,
                        style: {
                            shape: 'umlFrame', whiteSpace: 'wrap', html: 1, pointerEvents: 0,
                            gradientColor: '#f7ddb4', gradientDirection: 'east', fillColor: '#f7ddb4', strokeColor: '#000000', fontColor: '#000000'
                        }
                    },
                    {

                        id: 'UML4SysML::Activity_Operation',
                        Object_Type: 'Activity_Operation',
                        Stereotype: 'Activity_Operation',
                        idSeed: 1,
                        title: 'Operation',
                        width: 160,
                        height: 80,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        menuTitle: 'Operation',
                        style: {
                            shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center', fillColor: '#C2BEFE', strokeColor: '#000000'
                        }

                    }
                ]
            },
            {
                id: 'UML4SysML::ActivityParameter',
                Object_Type: 'ActivityParameter',
                Stereotype: "ActivityParameter",
                idSeed: 1,
                icon: Parameter,
                title: 'Parameter',
                width: 15,
                height: 15,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    html: 1, shape: 'mxgraph.sysml.port', html: 1, resizable: 0, sysMLPortType: 'empty', labelPosition: 'center', verticalLabelPosition: 'bottom', align: 'center', verticalAlign: 'top', fillColor: '#f8cecc', strokeColor: '#b85450'
                },
                value: 'ActivityParameter:Integer'
            },
            // {
            //     id: 'UML4SysML::ActivityParameter_Continuous',
            //     Object_Type: 'ActivityParameter',
            //     Stereotype: "continuous",
            //     idSeed: 1,
            //     icon: Parameter,
            //     title: 'Parameter (continuous)',
            //     width: 15,
            //     height: 15,
            //     nodeType: 'rectangle',
            //     dropAble: true, // 是否可以作为drop的对象
            //     style: {
            //         html: 1, shape: 'mxgraph.sysml.port', html: 1, resizable: 0, sysMLPortType: 'empty', labelPosition: 'center', verticalLabelPosition: 'bottom', align: 'center', verticalAlign: 'top', fillColor: '#f8cecc', strokeColor: '#b85450'
            //     },
            //     value: 'Continuous:Integer'
            // },
            // {
            //     id: 'UML4SysML::ActivityParameter_Discrete',
            //     Object_Type: 'ActivityParameter',
            //     Stereotype: "discrete",
            //     idSeed: 1,
            //     icon: Parameter,
            //     title: 'Parameter (discrete)',
            //     width: 15,
            //     height: 15,
            //     nodeType: 'rectangle',
            //     dropAble: true, // 是否可以作为drop的对象
            //     style: {
            //         html: 1, shape: 'mxgraph.sysml.port', html: 1, resizable: 0, sysMLPortType: 'empty', labelPosition: 'center', verticalLabelPosition: 'bottom', align: 'center', verticalAlign: 'top', fillColor: '#f8cecc', strokeColor: '#b85450'
            //     },
            //     value: 'Discrete:Integer'
            // },
            // {
            //     id: 'UML4SysML::ActivityParameter_Optional',
            //     Object_Type: 'ActivityParameter',
            //     Stereotype: "optional",
            //     idSeed: 1,
            //     icon: Parameter,
            //     title: 'Parameter (optional)',
            //     width: 15,
            //     height: 15,
            //     nodeType: 'rectangle',
            //     dropAble: true, // 是否可以作为drop的对象
            //     style: {
            //         html: 1, shape: 'mxgraph.sysml.port', html: 1, resizable: 0, sysMLPortType: 'empty', labelPosition: 'center', verticalLabelPosition: 'bottom', align: 'center', verticalAlign: 'top', fillColor: '#f8cecc', strokeColor: '#b85450'
            //     },
            //     value: 'Optional:Integer'
            // },
            // {
            //     id: 'UML4SysML::ActivityParameter_Probability',
            //     Object_Type: 'ActivityParameter',
            //     Stereotype: "probability",
            //     idSeed: 1,
            //     icon: Parameter,
            //     title: 'Parameter (probability)',
            //     width: 15,
            //     height: 15,
            //     nodeType: 'rectangle',
            //     dropAble: true, // 是否可以作为drop的对象
            //     style: {
            //         html: 1, shape: 'mxgraph.sysml.port', html: 1, resizable: 0, sysMLPortType: 'empty', labelPosition: 'center', verticalLabelPosition: 'bottom', align: 'center', verticalAlign: 'top', fillColor: '#f8cecc', strokeColor: '#b85450'
            //     },
            //     value: 'Probability:Integer'
            // },
            {
                id: 'UML4SysML::ActivityObject',
                Object_Type: 'Object',
                Stereotype: "ActivityObject",
                idSeed: 1,
                icon: Object_node,
                title: 'Object Node',
                width: 160,
                height: 80,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center', verticalAlign: 'top',
                    fillColor: '#afe6e9', gradientDirection: 'east', gradientColor: '#c8eef0', strokeColor: '#000000', fontColor: '#000000'
                }
            },
            // {
            //     id: 'UML4SysML::ActivityObject_Menu',
            //     icon: Object_node,
            //     title: 'Object Node (no buffer)',
            //     width: 20,
            //     height: 20,
            //     dropAble: true, // 是否可以作为drop的对象
            //     style: {
            //         fillColor: '#B3E7EA'
            //     },
            //     menuItem: [
            //         {

            //             id: 'UML4SysML::ActivityObject_noBuffer',
            //             Object_Type: 'Object',
            //             Stereotype: "noBuffer",
            //             idSeed: 1,
            //             icon: Object_node,
            //             title: 'NoBuffer',
            //             width: 160,
            //             height: 80,
            //             nodeType: 'rectangle',
            //             dropAble: true, // 是否可以作为drop的对象
            //             menuTitle: 'Stand-alone',
            //             NType: 0,
            //             style: {
            //                 shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center',
            //                 fillColor: '#afe6e9', gradientDirection: 'east', gradientColor: '#c8eef0', strokeColor: '#000000', fontColor: '#000000', verticalAlign: 'top'
            //             }
            //         },
            //         {

            //             id: 'UML4SysML::ActivityObject_EdgeMounted',
            //             Object_Type: 'ObjectNode',
            //             Stereotype: "noBuffer",
            //             idSeed: 1,
            //             icon: Parameter,
            //             title: 'NoBuffer',
            //             width: 15,
            //             height: 15,
            //             nodeType: 'rectangle',
            //             NType: 0,
            //             dropAble: true, // 是否可以作为drop的对象
            //             menuTitle: 'Edge Mounted',
            //             style: {
            //                 html: 1, shape: 'mxgraph.sysml.port', html: 1, resizable: 0, sysMLPortType: 'empty', labelPosition: 'center', verticalLabelPosition: 'bottom',
            //                 align: 'center', verticalAlign: 'top', fillColor: '#b1ddf0', strokeColor: '#10739e'
            //             },
            //             value: 'NoBuffer'
            //         },

            //     ]
            // },
            // {
            //     id: 'UML4SysML::ActivityObject_Overwrite_Menu',
            //     icon: Object_node,
            //     title: 'Object Node (overwrite)',
            //     width: 20,
            //     height: 20,
            //     dropAble: true, // 是否可以作为drop的对象
            //     style: {
            //         fillColor: '#B3E7EA'
            //     },
            //     menuItem: [
            //         {

            //             id: 'UML4SysML::ActivityObject_overwrite',
            //             Object_Type: 'Object',
            //             Stereotype: "overwrite",
            //             idSeed: 1,
            //             icon: Object_node,
            //             title: 'Overwrite',
            //             width: 160,
            //             height: 80,
            //             nodeType: 'rectangle',
            //             dropAble: true, // 是否可以作为drop的对象
            //             menuTitle: 'Stand-alone',
            //             NType: 0,
            //             style: {
            //                 shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center', verticalAlign: 'top',
            //                 fillColor: '#afe6e9', gradientDirection: 'east', gradientColor: '#c8eef0', strokeColor: '#000000', fontColor: '#000000'
            //             }

            //         },
            //         {

            //             id: 'UML4SysML::ActivityObject_overwrite_em',
            //             Object_Type: 'ObjectNode',
            //             Stereotype: "overwrite",
            //             idSeed: 1,
            //             icon: Parameter,
            //             title: 'Overwrite',
            //             width: 15,
            //             height: 15,
            //             nodeType: 'rectangle',
            //             dropAble: true, // 是否可以作为drop的对象
            //             menuTitle: 'Edge Mounted',
            //             NType: 0,
            //             style: {
            //                 html: 1, shape: 'mxgraph.sysml.port', html: 1, resizable: 0, sysMLPortType: 'empty', labelPosition: 'center', verticalLabelPosition: 'bottom', align: 'center', verticalAlign: 'top', fillColor: '#b1ddf0', strokeColor: '#10739e'
            //             },
            //             value: 'Overwrite'

            //         }
            //     ]
            // },
            {
                id: 'UML4SysML::CentralBufferNode',
                Object_Type: 'CentralBufferNode',
                Stereotype: "CentralBufferNode",
                idSeed: 1,
                icon: Central_buffer_node,
                title: 'Central Buffer Node',
                width: 160,
                height: 80,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                NType: 0,
                style: {
                    shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center', verticalAlign: 'top',
                    fillColor: '#bdd9f9', gradientDirection: 'east', gradientColor: '#daeafc', strokeColor: '#000000', fontColor: '#000000'
                }
            },
            {
                id: 'UML4SysML::Datastore',
                Object_Type: 'Object',
                Stereotype: "datastore",
                NType: 5,
                idSeed: 1,
                icon: Datastore,
                title: 'Datastore',
                width: 160,
                height: 80,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center', verticalAlign: 'top',
                    fillColor: '#bdd9f9', gradientDirection: 'east', gradientColor: '#daeafc', strokeColor: '#000000', fontColor: '#000000'
                }
            },
            {
                id: 'UML4SysML::Decision',
                Object_Type: 'Decision',
                Stereotype: null,
                NType: 0,
                idSeed: 1,
                icon: Decision,
                title: 'Decision',
                width: 40,
                height: 60,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    shape: 'rhombus', html: 1, dashed: 0, whiteSpace: 'wrap', perimeter: 'rhombusPerimeter',
                    labelPosition: 'center', verticalLabelPosition: 'bottom', align: 'center', verticalAlign: 'top',
                    fillColor: '#bfda88', gradientDirection: 'east', gradientColor: '#cce2a0', strokeColor: '#000000', fontColor: '#000000'
                }
            },
            {
                id: 'UML4SysML::MergeNode',
                Object_Type: 'MergeNode',
                Stereotype: null,
                idSeed: 1,
                icon: Merge,
                title: 'Merge',
                width: 40,
                height: 60,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    shape: 'rhombus', html: 1, dashed: 0, whiteSpace: 'wrap', perimeter: 'rhombusPerimeter',
                    labelPosition: 'center', verticalLabelPosition: 'bottom', align: 'center', verticalAlign: 'top',
                    fillColor: '#bfda88', gradientDirection: 'east', gradientColor: '#cce2a0', strokeColor: '#000000', fontColor: '#000000'
                }
            },
            {
                id: 'UML4SysML::StateNode',
                Object_Type: 'StateNode',
                Stereotype: null,
                NType: 6,
                idSeed: 1,
                icon: Synch,
                title: 'Synch',
                width: 40,
                height: 40,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    pointerEvents: 1, verticalLabelPosition: 'bottom', shadow: 0, dashed: 0, align: 'center', html: 1, verticalAlign: 'top', shape: 'mxgraph.electrical.signal_sources.source',
                    aspect: 'fixed', points: [[0.5, 0, 0], [1, 0.5, 0], [0.5, 1, 0], [0, 0.5, 0]], elSignalType: 'noise',
                    fillColor: '#bfda88', gradientDirection: 'east', gradientColor: '#cce2a0', strokeColor: '#000000', fontColor: '#000000'
                }
            },
            {
                id: 'UML4SysML::StateNode_ActivityInitial',
                Object_Type: 'StateNode',
                Stereotype: "StateNode_ActivityInitial",
                NType: 100,
                idSeed: 1,
                icon: Initial,
                title: 'Initial',
                width: 40,
                height: 40,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    shape: 'ellipse', html: 1, fillColor: 'strokeColor', strokeWidth: 2, perimeter: 'ellipsePerimeter',
                    verticalAlign: 'top', labelPosition: 'center', verticalLabelPosition: 'bottom', align: 'center', strokeColor: '#000000', fontColor: '#000000'
                },
                value: 'ActivityInitial'
            },
            {
                id: 'UML4SysML::StateNode_ActivityFinal',
                Object_Type: 'StateNode',
                Stereotype: "StateNode_ActivityFinal",
                NType: 101,
                idSeed: 1,
                icon: Final,
                title: 'Final',
                width: 40,
                height: 40,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    html: 1, strokeWidth: 1, shape: 'mxgraph.lean_mapping.sequenced_pull_ball', verticalAlign: 'top', labelPosition: 'center', verticalLabelPosition: 'bottom', align: 'center',
                    fillColor: '#bfda88', gradientDirection: 'east', gradientColor: '#cce2a0', strokeColor: '#000000', fontColor: '#000000'
                },
                value: 'ActivityFinal'
            },
            {
                id: 'UML4SysML::StateNode_FlowFinal',
                Object_Type: 'StateNode',
                Stereotype: "StateNode_FlowFinal",
                NType: 102,
                idSeed: 1,
                icon: Flow_final,
                title: 'Flow Final',
                width: 40,
                height: 40,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    shape: 'sumEllipse', perimeter: 'ellipsePerimeter', whiteSpace: 'wrap', html: 1, backgroundOutline: 1, verticalLabelPosition: 'bottom', verticalAlign: 'top', strokeWidth: 1,
                    fillColor: '#bfda88', gradientDirection: 'east', gradientColor: '#cce2a0', strokeColor: '#000000', fontColor: '#000000'
                },
                value: 'FlowFinal'
            },
            {
                id: 'UML4SysML::InterruptibleActivityRegion',
                Object_Type: 'InterruptibleActivityRegion',
                Stereotype: "InterruptibleActivityRegion",
                NType: 0,
                idSeed: 1,
                icon: Region,
                title: 'Region',
                width: 300,
                height: 200,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center', fillColor: 'none', dashed: 1, labelPosition: 'center', verticalLabelPosition: 'top', verticalAlign: 'bottom', spacing: -20, strokeColor: '#000000', fontColor: '#000000'
                },
                value: 'InterruptibleActivityRegion'
            },
            {
                id: 'UML4SysML::ExceptionHandler',
                Object_Type: 'ExceptionHandler',
                Stereotype: "ExceptionHandler",
                NType: 0,
                idSeed: 1,
                icon: Exception,
                title: 'ExceptionHandler',
                width: 160,
                height: 80,
                nodeType: 'rectangle',
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center',
                    fillColor: '#fae9db', gradientDirection: 'east', gradientColor: '#fefaf7', strokeColor: '#000000', fontColor: '#000000'
                },
                childElement: [
                    {
                        id: 'UML4SysML::ActivityParameter',
                        Object_Type: 'ObjectNode',
                        Stereotype: "ActivityParameter",
                        idSeed: 1,
                        icon: Parameter,
                        title: 'Parameter',
                        width: 15,
                        height: 15,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        style: {
                            html: 1, shape: 'mxgraph.sysml.port', html: 1, resizable: 0, sysMLPortType: 'empty', labelPosition: 'center', verticalLabelPosition: 'bottom', align: 'center', verticalAlign: 'top',
                            fillColor: '#c0dbf9', gradientDirection: 'east', gradientColor: '#daeafc', strokeColor: '#000000', fontColor: '#000000'
                        },
                        value: 'ActivityParameter:Integer'
                    }
                ]
            },
            {
                id: 'UML4SysML::Forck_Join',
                icon: Fork,
                title: 'Fork/Join...',
                width: 20,
                height: 20,
                dropAble: true, // 是否可以作为drop的对象
                style: {
                    fillColor: 'none'
                },
                menuItem: [
                    {

                        id: 'UML4SysML::Synchronization',
                        Object_Type: 'Synchronization',
                        Stereotype: "Synchronization",
                        NType: 0,
                        idSeed: 1,
                        icon: Object_node,
                        title: 'Synchronization',
                        width: 160,
                        height: 20,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        menuTitle: 'Horizontal',
                        style: {
                            shape: 'line', strokeWidth: 6, html: 1, strokeColor: '#000000', fontColor: '#000000', verticalAlign: 'top', labelPosition: 'center', verticalLabelPosition: 'bottom', align: 'center', horizontal: 1
                        },
                        value: 'Horizontal'
                    },
                    {

                        id: 'UML4SysML::Synchronization_Vertical',
                        Object_Type: 'Synchronization',
                        Stereotype: "Synchronization_Vertical",
                        NType: 1,
                        idSeed: 1,
                        icon: Object_node,
                        title: 'Vertical',
                        width: 20,
                        height: 160,
                        nodeType: 'rectangle',
                        dropAble: true, // 是否可以作为drop的对象
                        menuTitle: 'Vertical',
                        style: {
                            shape: 'line', strokeWidth: 6, html: 1, rotation: 90, strokeColor: '#000000', fontColor: '#000000', verticalAlign: 'top', labelPosition: 'center', verticalLabelPosition: 'bottom', align: 'center', horizontal: 1
                        },
                        value: 'Vertical'

                    }
                ]
            },
        ]
    },
    {
        Group: "SysML Activity Relationships",
        GroupAlias: "SysML Activity Relationships",
        items: [
            {
                id: 'UML4SysML::ControlFlow',
                Connector_Type: "ControlFlow",
                Stereotype: "ControlFlow",
                idSeed: 1,
                icon: Control_flow,
                title: 'Control Flow',
                width: 1,
                isEdge: true,
                isPort: false,
                nodeType: 'link',
                dropAble: false, // 是否可以作为drop的对象
                PDATA2: '',
                PDATA3: '',
                style: {
                    endArrow: 'open', endFill: 1, endSize: 12, html: 1, rounded: 0, strokeColor: '#000000'
                }
            },
            // {
            //     id: 'UML4SysML::ControlFlow_Continuous',
            //     Connector_Type: "ControlFlow",
            //     Stereotype: "continuous",
            //     idSeed: 1,
            //     icon: Control_flow,
            //     title: 'Control Flow (Continuous)',
            //     width: 1,
            //     isEdge: true,
            //     isPort: false,
            //     nodeType: 'link',
            //     dropAble: false, // 是否可以作为drop的对象
            //     style: {
            //         endArrow: 'open', endFill: 1, endSize: 12, html: 1, rounded: 0, strokeColor: '#000000'
            //     }
            // },
            // {
            //     id: 'UML4SysML::ControlFlow_Discrete',
            //     Connector_Type: "ControlFlow",
            //     Stereotype: "discrete",
            //     idSeed: 1,
            //     icon: Control_flow,
            //     title: 'Control Flow (Discrete)',
            //     width: 1,
            //     isEdge: true,
            //     isPort: false,
            //     nodeType: 'link',
            //     dropAble: false, // 是否可以作为drop的对象
            //     style: {
            //         endArrow: 'open', endFill: 1, endSize: 12, html: 1, rounded: 0, strokeColor: '#000000'
            //     }
            // },
            // {
            //     id: 'UML4SysML::ControlFlow_Probability',
            //     Connector_Type: "ControlFlow",
            //     Stereotype: "probability",
            //     idSeed: 1,
            //     icon: Control_flow,
            //     title: 'Control Flow (Probability)',
            //     width: 1,
            //     isEdge: true,
            //     isPort: false,
            //     nodeType: 'link',
            //     dropAble: false, // 是否可以作为drop的对象
            //     style: {
            //         endArrow: 'open', endFill: 1, endSize: 12, html: 1, rounded: 0, strokeColor: '#000000'
            //     }
            // },
            {
                id: 'UML4SysML::ObjectFlow',
                Connector_Type: "ObjectFlow",
                Stereotype: "ObjectFlow",
                idSeed: 1,
                icon: Object_flow,
                title: 'Object Flow',
                width: 1,
                isEdge: true,
                isPort: false,
                nodeType: 'link',
                dropAble: false, // 是否可以作为drop的对象
                style: {
                    endArrow: 'open', endFill: 1, endSize: 12, html: 1, rounded: 0, strokeColor: '#000000'
                },
                sourcePort: ObjectNodeSysML,
                targetPort: ObjectNodeSysML,
            },
            // {
            //     id: 'UML4SysML::ObjectFlow_Continuous',
            //     Connector_Type: "ObjectFlow",
            //     Stereotype: "continuous",
            //     idSeed: 1,
            //     icon: Object_flow,
            //     title: 'Object Flow (Continuous)',
            //     width: 1,
            //     isEdge: true,
            //     isPort: false,
            //     nodeType: 'link',
            //     dropAble: false, // 是否可以作为drop的对象
            //     style: {
            //         endArrow: 'open', endFill: 1, endSize: 12, html: 1, rounded: 0, strokeColor: '#000000'
            //     },
            //     sourcePort: ObjectNodeSysML,
            //     targetPort: ObjectNodeSysML,
            // },
            // {
            //     id: 'UML4SysML::ObjectFlow_Discrete',
            //     Connector_Type: "ObjectFlow",
            //     Stereotype: "discrete",
            //     idSeed: 1,
            //     icon: Object_flow,
            //     title: 'Object Flow (Discrete)',
            //     width: 1,
            //     isEdge: true,
            //     isPort: false,
            //     nodeType: 'link',
            //     dropAble: false, // 是否可以作为drop的对象
            //     style: {
            //         endArrow: 'open', endFill: 1, endSize: 12, html: 1, rounded: 0, strokeColor: '#000000'
            //     },
            //     targetPort: ObjectNodeSysML
            // },
            // {
            //     id: 'UML4SysML::ObjectFlow_Probability',
            //     Connector_Type: "ObjectFlow",
            //     Stereotype: "probability",
            //     idSeed: 1,
            //     icon: Object_flow,
            //     title: 'object Flow (Probability)',
            //     width: 1,
            //     isEdge: true,
            //     isPort: false,
            //     nodeType: 'link',
            //     dropAble: false, // 是否可以作为drop的对象
            //     style: {
            //         endArrow: 'open', endFill: 1, endSize: 12, html: 1, rounded: 0, strokeColor: '#000000'
            //     },
            //     sourcePort: ObjectNodeSysML,
            //     targetPort: ObjectNodeSysML,
            // },
            {
                id: 'UML4SysML::InterruptFlow',
                Connector_Type: "InterruptFlow",
                Stereotype: "InterruptFlow",
                idSeed: 1,
                icon: Interrupt_flow,
                title: 'Interrupt Flow',
                width: 1,
                isEdge: true,
                isPort: false,
                nodeType: 'link',
                dropAble: false, // 是否可以作为drop的对象
                style: {
                    shape: 'mxgraph.lean_mapping.electronic_info_flow_edge', endArrow: 'open', endFill: 1, endSize: 12, html: 1, rounded: 0, strokeColor: '#000000'
                }
            }
        ]
    },
    // {
    //     Group: "SysML Activity Extensions",
    //     GroupAlias: "SysML Activity Extensions",
    //     items: [
    //         {
    //             id: 'UML4SysML::Activity_effbd',
    //             Object_Type: 'Activity',
    //             Stereotype: "effbd",
    //             NType: 8,
    //             idSeed: 1,
    //             icon: Activity,
    //             title: 'Enhanced Functional Flow Block Diagrams',
    //             width: 160,
    //             height: 80,
    //             nodeType: 'rectangle',
    //             dropAble: true, // 是否可以作为drop的对象
    //             style: {
    //                 shape: 'umlState', rounded: 1, verticalAlign: 'top', spacingTop: 5, umlStateSymbol: 'collapseState', absoluteArcSize: 1, arcSize: 10, html: 1, whiteSpace: 'wrap',
    //                 gradientColor: '#fbf7db', gradientDirection: 'east', fillColor: '#f7f0bf', strokeColor: '#000000', fontColor: '#000000'
    //             }
    //         },
    //         {
    //             id: 'UML4SysML::Activity_streaming',
    //             Object_Type: 'Activity',
    //             Stereotype: "streaming",
    //             idSeed: 1,
    //             icon: Activity,
    //             title: 'Streaming Activity',
    //             width: 160,
    //             height: 80,
    //             nodeType: 'rectangle',
    //             dropAble: true, // 是否可以作为drop的对象
    //             style: {
    //                 shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center',
    //                 gradientColor: '#fbf7db', gradientDirection: 'east', fillColor: '#f7f0bf', strokeColor: '#000000', fontColor: '#000000'
    //             }
    //         },
    //         {
    //             id: 'UML4SysML::Activity_nonStreaming',
    //             Object_Type: 'Activity',
    //             Stereotype: "nonStreaming",
    //             idSeed: 1,
    //             icon: Activity,
    //             title: 'Non-Streaming Activity',
    //             width: 160,
    //             height: 80,
    //             nodeType: 'rectangle',
    //             dropAble: true, // 是否可以作为drop的对象
    //             style: {
    //                 shape: 'rect', html: 1, rounded: 1, whiteSpace: 'wrap', align: 'center', fillColor: '#fff2cc', strokeColor: '#000000',
    //                 gradientColor: '#fbf7db', gradientDirection: 'east', fillColor: '#f7f0bf', strokeColor: '#000000', fontColor: '#000000'
    //             }
    //         }
    //     ]
    // }
]
