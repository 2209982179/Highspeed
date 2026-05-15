<!-- 此组件通过解析标题文本内容，显示不同字体颜色；目的是实现将状态枚举解析成颜色文本的统一入口 -->
<template>
    <span v-if="text == '删除'" style="color: red">{{ text }}</span>
    <span v-else-if="text == '修改'" style="color: orange">{{ text }}</span>
    <span v-else-if="text == '移动'" style="color: orange">{{ text }}</span>
    <span v-else-if="text == '创建'" style="color: blue">{{ text }}</span>
    <span v-else-if="text == '新增'" style="color: blue">{{ text }}</span>
    <span v-else-if="text == '成功'" style="color: green">{{ text }}</span>
    <span v-else-if="text == '完成'" style="color: green">{{ text }}</span>
    <span v-else>{{ text }}</span>
</template>
<script>
export default {
    name: "StatusText",
    props: {
        text: {
            type: String,
            default: '',
        },
    },
};

</script>