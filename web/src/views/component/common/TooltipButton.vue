<template>
    <!-- 如果tip与title文本相同，认为无tip -->
    <el-tooltip v-if="tip && tip != title" :content="tip" effect="light" placement="bottom" :open-after="1500">
        <el-button v-if="disabled == true" :type="type" :plain="plain" class="is-disabled">{{ title }}</el-button>
        <el-button v-else @click="click" :type="type" :plain="plain">
            {{ title }}
        </el-button>
    </el-tooltip>
    <el-button v-else @click="click" :type="type" :plain="plain" :disabled="disabled">
        {{ title }}
    </el-button>
</template>

<script>

export default {
    name: "TooltipButton",
    props: {
        loading: {
            type: Boolean,
            default: false
        },
        disabled: {
            type: Boolean,
            default: false
        },
        plain: {
            type: Boolean,
            default: false
        },
        tip: {
            type: String,
            default: ''
        },
        type: {
            type: String,
            default: ''
        },
        title: {
            type: String,
            default: ''
        },
    },
    methods: {
        click() {
            this.$emit('click');
        },
    },
};
</script>
