import Allocation from './ContextMenu/Allocation'
export default {
    AddinList: [
        Allocation,
    ],
    ContextMenuItems: [],
    /**初始化 */
    InitContextMenuItems() {
        var subMenu = []
        var menu = []
        this.AddinList.forEach(addin => {
            addin.contextMenu().forEach(item => {
                // 未设置sort的时候往后排。
                item.sort = item.sort ?? 9999

                // 加入到children
                if (item.parentKey) {
                    subMenu.push(item)
                }else{
                    menu.push(item)
                }
            });
        });
        // 加入到children
        subMenu.forEach(item => {
            let parent = menu.find(x => x.key == item.parentKey)
            if (parent) {
                if (!parent.children) {
                    parent.children = []
                }
                parent.children.push(item)
            }
        });

        // 子菜单排序
        function sortMenu(menu) {
            menu = menu.sort((a, b) => a.sort - b.sort)
            menu.forEach(item => {
                if (item.children?.length > 0) {
                    sortMenu(item.children)
                }
            });
        }
        sortMenu(menu) 
        this.ContextMenuItems = menu
    },
}