/**会话数据缓存 */
import Vue from 'vue'
import {
  getAccessToken,
  removeAccessToken,
  setAccessToken,
} from '@/utils/accessToken'
import router from '@/router'

const state = () => ({
  loginUser: {},
  accessToken: getAccessToken(),
  userId: '',
  username: '',
  displayName: '',
  project: '',
  permissions: [],
  authManage: JSON.parse(sessionStorage.getItem('authManage') || '{}'),
})
const getters = {
  loginUser: (state) => state.loginUser,
  accessToken: (state) => state.accessToken,
  userId: (state) => state.userId,
  username: (state) => state.username,
  displayName: (state) => state.displayName,
  project: (state) => state.project,
  permissions: (state) => state.permissions,
  authManage: (state) => state.authManage,
}
const mutations = {
  setAccessToken(state, accessToken) {
    state.accessToken = accessToken
    setAccessToken(accessToken)
  },
  setUserInfo(state, user) {
    state.loginUser = user
    state.companyUid = user.companyUid
    state.userId = user.userId
    state.username = user.username
    state.displayName = user.displayName
    state.userType = user.userType
    state.avatar = user.avatar
    state.permissions = user.permissions
    state.DatabasePermissions = user.DatabasePermissions
    state.ProjectPermissions = user.ProjectPermissions
    state.SystemPermissions = user.SystemPermissions
  },
  setAuthManage(state, authManage) {
    state.authManage = { ...state.authManage, ...authManage }
    sessionStorage.setItem('authManage', JSON.stringify(state.authManage)) // 持久化
  },
  setProjectInfo(state, project) {
    state.project = project
  },
  setPermissions(state, permissions) {
    state.permissions = permissions
  },
}
const actions = {
  setPermissions({ commit }, permissions) {
    commit('setPermissions', permissions)
  },
  async login(userInfo) {    
      Vue.prototype.$message.success(`欢迎，${userInfo.username}!`) 
  },
  logout({ dispatch, state }) {
    state.projectInfo = {}
    dispatch('resetAccessToken').then(() => {
      router.replace({ path: '/login' }).then(() => {})
    })
  },
  resetAccessToken({ commit }) {
    commit('setPermissions', [])
    commit('setAccessToken', '')
    // 清除项目信息
    window.name = ''
    removeAccessToken()
  },
  refreshAccessToken({ commit }, accessToken) {
    if (accessToken) {
      removeAccessToken()
      commit('setAccessToken', accessToken)
    }
  },
}
export default { namespaced: true, state, getters, mutations, actions }
