#include<bits/stdc++.h>
using namespace std;
const int dir[4][2]={{0,1},{0,-1},{1,0},{-1,0}};
void swap(int& x,int& y){
    int t=x;
    x=y;
    y=t;
}
/*
f(n)=d(n)+h(n)
h1(n)=所有点绝对距离
*/

int cntSearch,cntExpand,maxDeep;    //以搜索节点，已扩展节点，搜索最大深度
int begFlag,endFlag;          //目标状态，初始状态
// int wzNow[10][2];
int wzNeed[10][2];
struct changeInfo{
    int x,y,val;
};
struct point{
    int dn,fn,par;  //某节点的d[n],f[n],父节点
    changeInfo bef; //原状态
    changeInfo aft; //现状态
};
unordered_map<int,point>info;   //记录状态的详细信息
unordered_map<int,int>vis;      //记录状态是否重复搜索

struct node
{
    int now,f;
    bool operator < (const node& b) const{
        return f > b.f;
    }
};
priority_queue<node> pq;

//计算h[n]
int calh(int use[3][3]){
    int ans=0;
    for(int i=0;i<3;i++){
        for(int j=0;j<3;j++){
            ans+=abs(i-wzNeed[use[i][j]][0])+abs(j-wzNeed[use[i][j]][1]);
        }
    }
    return ans;
}

//3*3数组转化为9位整数
int mat2int(int use[3][3]){
    int ans=0;
    for(int i=0;i<3;i++){
        for(int j=0;j<3;j++){
            ans=ans*10+use[i][j];
        }
    }
    return ans;
}

//9位整数转化为3*3数组
//n表示9位整数，(x,y)记录数字0的位置
void int2mat(int use[3][3],int n,int & x,int & y){
    for(int i=2;i>=0;i--){
        for(int j=2;j>=0;j--){
            use[i][j]=n%10;
            if(use[i][j]==0){
                x=i,y=j;
            }
            n/=10;     
        }
    }
}

void solve(){
    int use[3][3],now,dnnow,x,y,xx,yy;
    while(!pq.empty()){
        now=pq.top().now;
        dnnow=info[now].dn;
        pq.pop();
        if(vis.count(now)) //重复搜索，跳过
            continue;
        cntSearch++;    //已扩展节点数量增加
        maxDeep=max(maxDeep,info[now].dn);//搜索节点中深度最大值
        vis[now]=1;
        if(now==endFlag){   //已经是目标状态，返回
            return;
        }
        int2mat(use,now,x,y);   //获得数组形式的状态
        for(int i=0;i<4;i++){   //搜索子状态，利用数字0向上下左右四个方向搜索
            xx=x+dir[i][0],yy=y+dir[i][1];
            if(xx>=0 && xx<3 && yy>=0 && yy<3 ){//判断子状态合法性
                swap(use[x][y],use[xx][yy]);
                int num=mat2int(use);
                if(vis.count(num)==0 && (info.count(num)==0 || dnnow+1<info[num].dn)){
                    //子状态合法，加入队列
                    info[num]={dnnow+1,dnnow+1+calh(use),now,
                    {x,y,use[xx][yy]},{xx,yy,use[x][y]}};
                    pq.push({num,info[num].fn});
                    cntExpand++;
                }
                swap(use[x][y],use[xx][yy]);    //回溯
            }
        }
    }
}

//打印答案
stack<int>stk;
void printAns(double usetime){
    FILE* fp = fopen("D:\\out.txt","w");
    FILE* fp1 = fopen("D:\\cread.txt","w");

    if(fp==NULL){
        printf("open error");
        exit(0);
    }

    int now=endFlag;
    while(info[now].par){
        stk.push(now);
        now=info[now].par;
    }
    stk.push(now);
    // printf("%d\n",stk.size());

    // int n;
    // scanf("%d",&n);
    fprintf(fp1,"%d\n%d\n%d\n%d\n%d\n%.6lfs\n",begFlag,endFlag,cntSearch,cntExpand,maxDeep,usetime);

    fprintf(fp,"Begin:%d\n",begFlag);
    fprintf(fp,"End:%d\n",endFlag);
    fprintf(fp,"Search:%d\n",cntSearch);
    fprintf(fp,"Expand:%d\n",cntExpand);
    fprintf(fp,"Maxdeep:%d\n",maxDeep);
    fprintf(fp,"Runtime:%.6lfs\n",usetime);


    int cnt=1,ansnow;
    int x,y;
    fprintf(fp,"no\tstatus\t\td[n]\tf[n]\tchange information\n");
    while(!stk.empty()){
        ansnow=stk.top();
        fprintf(fp,"%d\t%d\t%d\t\t%d\t\t",cnt++,ansnow,info[ansnow].dn,info[ansnow].fn);
        fprintf(fp,"%d,%d %d-->%d,%d %d\n",info[ansnow].bef.x,info[ansnow].bef.y,
        info[ansnow].bef.val,info[ansnow].aft.x,info[ansnow].aft.y,
        info[ansnow].aft.val);
        fprintf(fp1,"%d\n",ansnow);
        // int use[3][3];
        // int2mat(use,ansnow,x,y);
        // for(int i=0;i<3;i++){
        //     for(int j=0;j<3;j++){
        //         printf("%d",use[i][j]);
        //     }
        //     printf("\n");
        // }
        // printf("%d\n",calh(use));
        // if(cnt%10==0) scanf("%d",&n);
        stk.pop();
    }
}

int main(int argc, char **argv){
    int n,m,num;
    // cout<<"input begin status(9 continue numbers)\n";
    // cin>>n;
    // n=123485706;
    n=atoi(argv[1]);
    begFlag=n;
    

    // cout<<"input end status(9 continue numbers)\n";
    // cin>>m;
    // m=123456780;
    m=atoi(argv[2]);
    endFlag=m;

    // cout<<n<<" "<<m;

    endFlag=m;
    for(int i=2;i>=0;i--){
        for(int j=2;j>=0;j--){
            num=m%10;
            m/=10;
            wzNeed[num][0]=i;
            wzNeed[num][1]=j;
        }
    }

    
    int use[3][3],x,y;
    int2mat(use,n,x,y);
    info[n]={1,1+calh(use),0};
    pq.push({n,info[n].fn});

    // for(int i=0;i<=9;i++){
    //     printf("%d %d %d\n",i,wzNeed[i][0],wzNeed[i][1]);
    // }
    clock_t startTime,endTime;
    startTime = clock();
    solve();
    endTime = clock();
    double usetime=(double)(endTime - startTime) / CLOCKS_PER_SEC;
    printAns(usetime);

    return 0;
}

/*
716325480
812657340
263841750
423176580
*/

