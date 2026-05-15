<template>
  <div class="login-container">
    <el-container style="justify-content: right">
      <el-container style="justify-content: center; width: 40%">
        <div class="login-form-box">
          <el-form
            ref="form"
            :model="form"
            :rules="rules"
            class="login-form"
            label-position="left"
          >
            <div class="title-tips wrap-row">登录页面</div>
            <el-form-item prop="username" style="margin-top: 40px">
              <span class="svg-container svg-container-admin">
                <i class="el-icon-monitor" />
              </span>
              <el-input
                v-model.trim="form.username"
                v-focus
                placeholder="用户名"
                tabindex="1"
                type="text"
              />
            </el-form-item>
            <el-form-item prop="password">
              <span class="svg-container">
                <i class="el-icon-key" />
              </span>
              <el-input
                :key="passwordType"
                ref="password"
                v-model.trim="form.password"
                placeholder="密码"
                :type="passwordType"
                tabindex="2"
                @keyup.enter.native="handleLogin"
              />
              <span
                v-if="passwordType === 'password'"
                class="show-password"
                @click="handlePassword"
              >
                <i class="el-icon-view" />
              </span>
              <span v-else class="show-password2" @click="handlePassword">
                <i class="el-icon-view" />
              </span>
            </el-form-item>
            <el-form-item>
              <el-link type="primary" @click="graghTest">图形测试</el-link>
              <el-button
                :loading="loading"
                class="login-btn"
                type="primary"
                @click="handleLogin"
              >
                登 录
              </el-button>
            </el-form-item>
          </el-form>
        </div>
      </el-container>
    </el-container>
  </div>
</template>

<script>
  export default {
    name: 'Login',
    directives: {
      focus: {
        inserted(el) {
          el.querySelector('input').focus()
        },
      },
    },
    data() {
      const validateusername = (rule, value, callback) => {
        if ('' === value) {
          callback(new Error('用户名填写错误'))
        } else {
          callback()
        }
      }
      const validatePassword = (rule, value, callback) => {
        callback()
      }
      return {
        nodeEnv: process.env.NODE_ENV,
        form: {
          username: '',
          password: '',
        },
        rules: {
          username: [
            {
              required: true,
              trigger: 'blur',
              validator: validateusername,
            },
          ],
          password: [
            {
              required: true,
              trigger: 'blur',
              validator: validatePassword,
            },
          ],
        },
        loading: false,
        passwordType: 'password',
        redirect: undefined,
      }
    },
    watch: {
      $route: {
        handler(route) {
          this.redirect = (route.query && route.query.redirect) || '/'
        },
        immediate: true,
      },
    },
    created() {
      document.body.style.overflow = 'hidden'
    },
    beforeDestroy() {
      document.body.style.overflow = 'auto'
    },
    methods: {
      handleShowAbout() {
        this.$refs.about.show()
      },
      handlePassword() {
        this.passwordType === 'password'
          ? (this.passwordType = '')
          : (this.passwordType = 'password')
        this.$nextTick(() => {
          this.$refs.password.focus()
        })
      },
      handleLogin() {
        this.$refs.form.validate((valid) => {
          if (valid) {
            this.loading = true
            this.$store
              .dispatch('user/login', this.form)
              .then(() => {
                if (this.redirect === '/404' || this.redirect === '/401') {
                  this.$router.push('/').catch(() => {})
                  this.loading = false
                } else {
                  this.$message.success(`登录完成，${this.form.username}`)
                  this.$router.push('/TestCaseList').catch(() => {})
                  this.loading = false
                }
              })
              .catch((err) => {
                this.$message.error(err)
                this.loading = false
              })
          } else {
            return false
          }
        })
      },
      graghTest() {
        this.$message.success(`加载完成，${this.form.username}`)
        this.$router.push('/TestCaseDraw').catch(() => {})
      },
    },
  }
</script>

<style lang="scss" scoped>
  .login-container {
    height: 100vh;
    background: url('~@/assets/images/loginBG1.jpg') center center fixed
      no-repeat;
    background-size: cover;

    .title {
      font-size: 54px;
      font-weight: 600;
      color: #333333;
    }

    .title-tips {
      margin-left: 7%;
      margin-top: 29px;
      font-size: 32px;
      font-weight: 600;
      font-family: '宋体';
      color: #333333;
      text-overflow: ellipsis;
      width: 100%;
      height: 2em;
      white-space: normal;
      word-break: break-word;
    }

    .login-btn {
      display: inherit;
      background-color: #666666;
      float: right;
      width: 180px;
      height: 48px;
      margin: 5px 0px;
      border: 0;

      &:hover {
        opacity: 0.9;
      }
    }

    .login-form-box {
      position: absolute;
      border-radius: 6px;
      top: 50%;
      left: 60%;
      transform: translate(10%, -50%);
      justify-content: right;
      width: 30%;
      min-width: 250px;
      max-width: 550px;

      &::after {
        position: absolute;
        content: '';
        margin: -3px;
        border-radius: 10px;
        background-image: linear-gradient(
          -135deg,
          transparent -30px,
          #a1a1a1,
          #666666,
          #9a9a9a,
          #333333,
          #dcdcdc
        );
        inset: 0;
        filter: blur(5px);
        z-index: -1;
        animation: rotate 5s infinite linear;
      }

      @keyframes rotate {
        from {
          filter: hue-rotate(0deg) blur(5px);
        }

        to {
          filter: hue-rotate(360deg) blur(5px);
        }
      }
    }

    .login-form {
      background: url('~@/assets/images/loginBG2.jpg') top right fixed no-repeat;
      border: 3px solid #dcdcdc;
      border-radius: 8px;
      padding: 30px 30px 0 30px;
    }

    .svg-container {
      position: absolute;
      top: 4px;
      left: 10px;
      z-index: 1;
      font-size: 20px;
      color: #666666;
      cursor: pointer;
      user-select: none;
    }

    .show-password {
      position: absolute;
      top: 4px;
      right: 10px;
      font-size: 20px;
      color: #666666;
      cursor: pointer;
      user-select: none;
    }

    .show-password2 {
      position: absolute;
      top: 4px;
      right: 10px;
      font-size: 20px;
      color: #2266ee;
      cursor: pointer;
      user-select: none;
    }

    ::v-deep {
      .el-form-item {
        padding-right: 0;
        margin: 30px 30px;
        color: #454545;
        background: transparent;
        border: 1px solid transparent;
        border-radius: 2px;

        &__content {
          min-height: 30px;
          line-height: 30px;
        }

        &__error {
          position: absolute;
          top: 100%;
          left: 18px;
          font-size: 14px;
          line-height: 22px;
          color: #ff0000;
        }
      }

      .el-button {
        ::before {
          color: #ffffff;
        }

        span {
          font-size: 16px;
          font-weight: 600;
        }
      }

      .el-input {
        box-sizing: border-box;

        &__inner {
          height: 42px !important;
        }

        input {
          height: 42px;
          padding-left: 40px;
          padding-right: 40px;
          font-size: 20px;
          line-height: 42px;
          color: #1c1c1c;
          background: #ffffff;
          border: 1 solid #c1c1c1;
          border-radius: 3px;
          caret-color: #1c1c1c;
        }
      }

      .register-btn {
        padding: 20px 0px 0px 0px !important;
        margin-left: 0 !important;

        span {
          font-size: 14px !important;
        }
      }
    }

    .lang-change {
      justify-content: right;
      height: 22px;
      font-size: 22px;
      padding: 15px;
    }

    .about {
      font-size: 22px !important;
      width: 22px;
      height: 22px;
      padding: 0;
      margin: 0;
      border: 0;
      background: transparent;
    }
  }

  .contact-area {
    justify-content: center;
    display: grid;
    align-items: end;
    width: 40%;
    height: 100vh;
    margin-top: -50px;
    pointer-events: none;

    .subtitle {
      text-align: center;
    }

    .contact {
      margin: 20px 0;
      line-height: 1.3em;
      pointer-events: auto;
    }

    .copyright {
      margin-top: 20px;
      color: #a1a1a1;
      font-size: 12px;
      line-height: 1.3em;
    }
  }
</style>
