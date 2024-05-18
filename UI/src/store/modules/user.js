import { login, logout, getInfo } from '@/api/user'
import { getToken, setToken, removeToken } from '@/utils/auth'
import { resetRouter } from '@/router'
import { ref } from 'vue';

const getDefaultState = () => {
  return {
    token: getToken(),
    name: '',
    avatar: ''
  }
}

const state = ref(getDefaultState());

const mutations = {
  RESET_STATE: () => {
    state.value = getDefaultState();
  },
  SET_TOKEN: (token) => {
    state.value.token = token;
  },
  SET_NAME: (name) => {
    state.value.name = name;
  },
  SET_AVATAR: (avatar) => {
    console.log('setAvatar:', avatar);
    state.value.avatar = avatar;
  }
}

const actions = {
  // user login
  login({ commit }, userInfo) {
    const { userName, password } = userInfo;
    return new Promise((resolve, reject) => {
      login({ userName: userName.trim(), password: password })
        .then(response => {
          const { data } = response;
          commit('SET_TOKEN', data.token);
          setToken(data.token);
          resolve();
        })
        .catch(error => {
          reject(error);
        })
    })
  },

  // get user info
  getInfo({ commit, state }) {
    return new Promise((resolve, reject) => {
      getInfo(state.value.token)
        .then(response => {
          const { data } = response;

          if (!data) {
            return reject('Verification failed, please Login again.');
          }

          // 只有名称，没有姓名，所以用 userId 代表
          const { userId, avatar } = data;

          commit('SET_NAME', userId);
          commit('SET_AVATAR', avatar);
          resolve(data);
        })
        .catch(error => {
          reject(error);
        })
    })
  },

  // user logout
  logout({ commit, state }) {
    return new Promise((resolve, reject) => {
      logout(state.value.token)
        .then(() => {
          removeToken(); // must remove  token  first
          resetRouter();
          commit('RESET_STATE');
          resolve();
        })
        .catch(error => {
          reject(error);
        })
    })
  },

  // remove token
  resetToken({ commit }) {
    return new Promise(resolve => {
      removeToken(); // must remove  token  first
      commit('RESET_STATE');
      resolve();
    })
  },

  // 设置头像
  setAvatar({ commit }, avatar) {
    console.log('setAvatar:', avatar);
    commit('SET_AVATAR', avatar);
  }
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}
