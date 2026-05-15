export default {
  contextMenu() {
    return [
      {
        key: 'Allocation.group',
        // 目标类型
        group: '基础功能菜单',
        sort: 9,
        title: '测试1',
        condition(source, target) {
          return (
            target == 'diagramEditor' &&
            (source.type == 'Diagram' || source.type == 'Element')
          )
        },
        // 是否可用
        enable: (source) =>
          [true, 'true'].includes(source.Readonly) ? false : true,
      },
    ]
  },
}
