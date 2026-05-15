<!-- 自定义风格 -->
<template>
    <table cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td class="group1 unselectable" style="background-color: #eee;" colspan="10"
                    @click="() => { isShow = !isShow }">
                    <i class="el-icon-s-grid" />
                    自定义风格
                    <i :class="{ 'el-icon-arrow-down': isShow, 'el-icon-arrow-up': !isShow }" style="float: right;" />
                </td>
            </tr>
            <tr>
                <td colspan="10">
                    <el-tooltip content="按照MDG类型生成风格" effect="light">
                        <div v-show="isShow" class="style-item"
                            :style="`border: 1px solid #909090; background-color: #f4f4f4;color: #000;`"
                            @click="setMDGStyle()">
                            <i class="el-icon-s-grid" style="margin-right: 6px;"></i>
                            <span>MDG</span>
                        </div>
                    </el-tooltip>
                    <div v-show="isShow" v-for="(item, index) in styleList" :key="index" class="style-item"
                        :style="`border: 1px solid ${item.strokeColor}; background-color: ${item.fillColor};color: ${item.fontColor};`"
                        @click="setStyle(item)">
                        {{ item.key }}</div>
                </td>
            </tr>
        </tbody>
    </table>
</template>

<script>
import mxgraph from '@/mxgraph/graph'
const { mxUtils } = mxgraph
import {
    GetMDGStyleByStereotype,
} from '@/API/Diagram'

// 仅修改个别样式，避免过多影响导致显示不符合预期
const canSetStyleKeys = ['strokeColor', 'fillColor', 'fontColor'];

export default {
    props: {
        currentDiagram: null,
        currentSelectedDiagramLinks: null,
        currentSelectedDiagramObjects: null,
    },
    data() {
        return {
            isShow: true,
            styleList: [
                { key: '普通', strokeColor: '#ccc', fillColor: '#fff', fontColor: '#774400' },
                { key: '主要', strokeColor: '#10739e', fillColor: '#b1ddf0', fontColor: '#000' },
                { key: '成功', strokeColor: '#82b366', fillColor: '#d5e8d4', fontColor: '#000' },
                { key: '警告', strokeColor: '#b46504', fillColor: '#fad7ac', fontColor: '#000' },
                { key: '危险', strokeColor: '#B20000', fillColor: '#e51400', fontColor: '#fff' },
            ],
        }
    },
    methods: {
        /** 设置指定样式 */
        setStyle(styleInfo) {
            let graph = this.currentDiagram?.graph
            if (!graph) {
                return
            }
            this.currentDiagram?.graph.model.beginUpdate()
            if (this.currentSelectedDiagramLinks?.length > 0) {
                // 连线样式
                this.currentSelectedDiagramLinks.forEach((item) => {
                    let style = item.style
                    Object.keys(styleInfo).forEach((key) => {
                        if (canSetStyleKeys.includes(key)) {
                            style = mxUtils.setStyle(style, key, styleInfo[key])
                        }
                    })
                    graph.model.setStyle(item, style)
                })
            }
            if (this.currentSelectedDiagramObjects?.length > 0) {
                // 图元样式
                this.currentSelectedDiagramObjects.forEach((item) => {
                    let style = item.style
                    Object.keys(styleInfo).forEach((key) => {
                        if (canSetStyleKeys.includes(key)) {
                            style = mxUtils.setStyle(style, key, styleInfo[key])
                        }
                    })
                    graph.model.setStyle(item, style)
                })
            }
            this.currentDiagram?.graph.model.endUpdate()
        },
        async setMDGStyle() {
            let graph = this.currentDiagram?.graph
            if (!graph) {
                return
            }
            this.currentDiagram?.graph.model.beginUpdate()
            // 图元
            let cells = this.currentSelectedDiagramObjects
            let objcetStereotypes = cells?.map(x => x.Stereotype) ?? []
            if (objcetStereotypes.length > 0) {
                // 去重，获取MDG样式
                let dict = await GetMDGStyleByStereotype([...new Set(objcetStereotypes)])
                // 更新样式
                dict.forEach(tuple => {
                    if (tuple.Item2) {
                        cells.filter(x => x.Stereotype == tuple.Item1).forEach(x => {
                            let style = x.style
                            let styles = this.$DiagramUtils.StyleSerialize(tuple.Item2, true)
                            styles.forEach((item) => {
                                if (canSetStyleKeys.includes(item.key)) {
                                    style = mxUtils.setStyle(style, item.key, item.value)
                                }
                            })
                            graph.model.setStyle(x, style)
                        })
                    }
                });
            }

            // 连线
            let edges = this.currentSelectedDiagramLinks
            let linkStereotypes = edges?.map(x => x.Stereotype) ?? []
            if (linkStereotypes?.length > 0 || objcetStereotypes.length > 0) {
                // 去重，获取MDG样式
                let dict = await GetMDGStyleByStereotype([...new Set(linkStereotypes)])
                // 更新样式
                dict.forEach(tuple => {
                    if (tuple.Item2) {
                        edges.filter(x => x.Stereotype == tuple.Item1).forEach(x => {
                            let style = x.style
                            let styles = this.$DiagramUtils.StyleSerialize(tuple.Item2, true)
                            styles.forEach((item) => {
                                if (canSetStyleKeys.includes(item.key)) {
                                    style = mxUtils.setStyle(style, item.key, item.value)
                                }
                            })
                            graph.model.setStyle(x, style)
                        })
                    }
                });
            }
            this.currentDiagram?.graph.model.endUpdate()
        },
    },
}
</script>
<style lang="scss" scoped>
@import 'table.scss';
@import 'index.scss';

/* 预设样式div */
.style-item {
    height: 40px;
    line-height: 40px;
    text-align: center;
    margin: 4px;
    width: 100px;
    cursor: pointer;
    float: left;
}
</style>