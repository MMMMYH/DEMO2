-- YouWanna 主入口文件
-- 使用 Cocos2d-x + Lua 开发

local function main()
    print("=== YouWanna 游戏启动 ===")
    
    -- 初始化显示
    local director = cc.Director:getInstance()
    local glview = director:getOpenGLView()
    
    if not glview then
        glview = cc.GLViewImpl:create("YouWanna", cc.size(800, 600))
        director:setOpenGLView(glview)
    end
    
    -- 设置设计分辨率
    glview:setDesignResolutionSize(800, 600, cc.ResolutionPolicy.SHOW_ALL)
    
    -- 显示FPS
    director:setDisplayStats(true)
    
    -- 设置FPS
    director:setAnimationInterval(1.0 / 60)
    
    -- 创建第一个场景
    local scene = require("src.GameScene"):create()
    director:runWithScene(scene)
    
    print("游戏初始化完成!")
end

-- 启动游戏
main()
