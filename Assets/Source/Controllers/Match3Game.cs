public class Match3Game
{
    private Board _board;
    
    private enum EState
    {
        WaitingToSwap,
        Swapping,
        CheckingMatches,
        SwappingBack,
        RemovingMatchPieces,
        MovingDownPieces,
    }

    private EState _currentState;
    private Matches _currentMatches;

    public Match3Game(Board board)
    {
        _board = board;
        _currentState = EState.WaitingToSwap;
    }

    public void Process()
    {
        UnityEngine.Debug.Log(_currentState);
        switch (_currentState)
        {
            case EState.WaitingToSwap:
                if (!_board.IsReadyToSwap)
                {
                    return;
                }

                _board.SwapCandidates();
                _currentState = EState.CheckingMatches;
                break;

            case EState.CheckingMatches:
                _currentMatches = _board.GetMatchesFromMovedPieces();
                if (_currentMatches.Count <= 0)
                {
                    if (_board.IsSwapping)
                    {
                        _currentState = EState.SwappingBack;
                    }
                    else
                    {
                        _board.ConfirmMovedPieces();
                        _currentState = EState.WaitingToSwap;
                    }
                }
                else
                {
                    if (_board.IsSwapping)
                    {
                        _board.ConfirmSwappedPieces();
                    }
                    _currentState = EState.RemovingMatchPieces;
                }
                break;

            case EState.SwappingBack:
                _board.SwapCandidates();
                _board.ConfirmMovedPieces();
                _currentState = EState.WaitingToSwap;
                break;

            case EState.RemovingMatchPieces:
                _board.RemovePiecesFromMatches(_currentMatches);
                _currentState = EState.MovingDownPieces;
                break;

            case EState.MovingDownPieces:
                _board.MovePiecesDown();
                _currentState = EState.CheckingMatches;
                break;
        }
    }
}
