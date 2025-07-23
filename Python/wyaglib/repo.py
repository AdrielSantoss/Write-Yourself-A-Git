import os
import configparser

class GitAdrRepository(object):
    worktree = None
    gitdir = None
    conf = None

    def __init__(self, path, force=False):
        self.worktree = path
        self.gitdir = os.path.join(path, '.adr')

        if not (force or os.path.isdir(self.gitdir)):
            raise Exception(f"Não é um repositório Git {path}")
        
        self.conf = configparser.ConfigParser()
        cf = repo_file(self, "config")

        if cf and os.path.exists(cf):
            self.conf.read([cf])
        elif not force:
            raise Exception("Configuração inválida.")
        
        
def repo_path(repo, *path):  # Monta e retorna um caminho completo dentro da pasta .grit do repositório.
    return os.path.join(repo.gitdir, *path)

def repo_file(repo, *path, mkdir=False): # Garante que o diretório pai do arquivo exista (criando se mkdir=True) e retorna o caminho
    if repo_dir(repo, *path[:-1], mkdir=mkdir):
        return repo_path(repo, *path)

def repo_dir(repo, *path, mkdir=False): # Garante que um diretório dentro do repositório exista.
    path = repo_path(repo, *path)
    
def repo_create(path):
    repo = GitAdrRepository(path, True)

    if os.path.exists(repo.gitdir):
        if not os.path.isdir(repo.gitdir):
            raise Exception(f"{repo.gitdir} não é um diretório")
    else:
        os.makedirs(repo.gitdir)

    # Inicializa estrutura
    assert repo_dir(repo, "branches", mkdir=True)
    assert repo_dir(repo, "objects", mkdir=True)
    assert repo_dir(repo, "refs", "tags", mkdir=True)
    assert repo_dir(repo, "refs", "heads", mkdir=True)

    # HEAD
    with open(repo_file(repo, "HEAD"), "w") as f:
        f.write("ref: refs/heads/master\n")

    # Configuração padrão
    with open(repo_file(repo, "config"), "w") as f:
        config = default_config()
        config.write(f)

    return repo  
