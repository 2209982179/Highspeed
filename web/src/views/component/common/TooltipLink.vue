<!-- 此组件基于 el-link el-tooltip 实现带tip的超链接文本 -->
<template>
    <el-tooltip v-if="tip" :content="tip" effect="light" placement="bottom" :disabled="['', null, undefined].includes(tip)">
        <el-link :icon="icon" :type="type" @click="click" :disabled="disabled">{{text}}</el-link>
    </el-tooltip>
    <el-link v-else :icon="icon" :type="type" @click="click" :disabled="disabled">{{text}}</el-link>
</template>

<script>

export default {
    name: "TooltipLink",
    props: {
        text: {
            type: String,
            default: '',
        },
        tip: {
            type: String,
            default: '',
        },
        icon: {
            type: String,
            default: '',
        },
        type: {
            type: String,
            default: 'primary',
        },
        disabled: {
            type: Boolean,
            default: false,
        },
    },
    data() {
        return {
        };
    },
    methods: {
        click() {
            this.$emit('click');
        }
    },
};

</script>
