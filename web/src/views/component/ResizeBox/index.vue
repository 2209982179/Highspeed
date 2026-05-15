<template>
    <!-- 在右侧提供宽度变更拖曳 -->
    <div v-if="resize === 'right'" class="container-right">
        <!-- resizable 用于拖拽的工具 -->
        <div class="resizable" :style="`min-width: ${minWidth}px;max-width: ${maxWidth}px;`"></div>
        <!-- content 要展示的内容区域 -->
        <div class="content">
            <component :is="context"></component>
        </div>
    </div>
    <!-- 在右侧提供宽度变更拖曳 -->
    <div v-else-if="resize === 'left'" class="container-left">
        <!-- resizable 用于拖拽的工具 -->
        <div class="resizable" :style="`min-width: ${minWidth}px;max-width: ${maxWidth}px;`"></div>
        <!-- content 要展示的内容区域 -->
        <div class="content">
            <component :is="context"></component>
        </div>
    </div>
    <!-- 在下面提供高度变更拖曳 -->
    <div v-else-if="resize === 'left'" class="container-bottom">
        <!-- resizable 用于拖拽的工具 -->
        <div class="resizable" :style="`min-width: ${minWidth}px;max-width: ${maxWidth}px;`"></div>
        <!-- content 要展示的内容区域 -->
        <div class="content">
            <component :is="context"></component>
        </div>
    </div>
</template>
  
<script>
import projectTree from '@/views/component/ProjectTreeView'
export default {
    data() {
        return {
        }
    },
    props: {
        resize: {
            type: String,
            default: 'right',
        },
        minWidth: {
            type: Number,
            default: 280,
        },
        maxWidth: {
            type: Number,
            default: 600,
        },
        context: {
            type: Object,
            default: null,
        },
    },
    components: {
        projectTree,
    },
}
</script>

<style lang="scss" scoped>
.container-right {
    position: relative;
    float: left;

    /*  */
    .resizable {
        height: 100vh;
        overflow: scroll;
        resize: horizontal;
        cursor: ew-resize;
        opacity: 0;
    }

    /* 更改拖拽图标的大小和父容器一样大 */
    .resizable::-webkit-scrollbar {
        width: 20px;
        height: 100vh;
    }

    /* 使用定位, 将容器定位到父容器的正中间, 跟着父容器的大小改变而改变 */
    .content {
        margin: 0;
        position: absolute;
        top: 0;
        /* 留出5px为了鼠标放上去可以显示拖拽 */
        right: 5px;
        bottom: 0;
        left: 0;
    }
}
.container-left {
    position: relative;
    float: right;
    
    /*  */
    .resizable {
        height: 100vh;
        overflow: scroll;
        resize: horizontal;
        cursor: ew-resize;
        opacity: 0;
        transform: scale(-1, 100);
    }

    /* 更改拖拽图标的大小和父容器一样大 */
    .resizable::-webkit-scrollbar {
        width: 20px;
        height: 100vh;
    }

    /* 使用定位, 将容器定位到父容器的正中间, 跟着父容器的大小改变而改变 */
    .content {
        position: absolute;
        margin: 0;
        top: 0;
        /* 留出5px为了鼠标放上去可以显示拖拽 */
        left: 5px;
        bottom: 0;
        right: 0;
    }
}
</style>