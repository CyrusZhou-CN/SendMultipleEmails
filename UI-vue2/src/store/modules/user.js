import { logout } from '@/api/user'
import { getToken, setToken, removeToken } from '@/utils/auth'
import { resetRouter } from '@/router'

const getDefaultState = () => {
  return {
    token: getToken(),
    userId: '',
    avatar: ''
  }
}

const state = getDefaultState()

const mutations = {
  RESET_STATE: state => {
    Object.assign(state, getDefaultState())
  },
  SET_TOKEN: (state, token) => {
    // console.log('setToken:', token)
    state.token = token
  },
  SET_USERID: (state, userId) => {
    state.userId = userId
  },
  SET_AVATAR: (state, avatar) => {
    console.log('setAvatar:', avatar)
    state.avatar = avatar
  }
}

const actions = {
  // user login
  setToken({ commit }, token) {
    // console.log('setToken:', token)
    commit('SET_TOKEN', token)
    setToken(token)
  },

  // 设置用户信息
  setUserInfo({ commit }, userInfo) {
    // 只有名称，没有姓名，所以用 userId 代表
    const { userId, avatar } = userInfo
    commit('SET_USERID', userId)
    commit('SET_AVATAR', avatar)
  },

  // user logout
  logout({ commit, state }) {
    return new Promise((resolve, reject) => {
      logout(state.token)
        .then(() => {
          removeToken() // must remove  token  first
          resetRouter()
          commit('RESET_STATE')
          resolve()
        })
        .catch(error => {
          reject(error)
        })
    })
  },

  // remove token
  resetToken({ commit }) {
    return new Promise(resolve => {
      removeToken() // must remove  token  first
      commit('RESET_STATE')
      resolve()
    })
  },

  // 设置头像
  setAvatar({ commit, state }, avatar) {
    console.log('setAvatar:', avatar)
    commit('SET_AVATAR', avatar)
  }
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}
