import {useCallback} from 'react';
import {
    addEdge,
    Background,
    BackgroundVariant,
    Connection,
    Edge,
    MiniMap,
    Node,
    ReactFlow,
    useEdgesState,
    useNodesState,
} from '@xyflow/react';

import '@xyflow/react/dist/style.css';


const initialNodes: Node[] = [
    {id: '1', position: {x: 0, y: 0}, data: {label: '1'}},
    {id: '2', position: {x: 0, y: 100}, data: {label: '2'}},
];

const initialEdges: Edge[] = [
    {id: 'e1-2', source: '1', target: '2'},
];

const App = () => {
    const [nodes, _, onNodesChange] = useNodesState(initialNodes);
    const [edges, setEdges, onEdgesChange] = useEdgesState(initialEdges);

    const onConnect = useCallback(
        (params: Edge<any> | Connection) =>
            setEdges((eds) => addEdge(params, eds)),
        [setEdges]
    );

    return (
        <div style={{width: '100vw', height: '100vh'}}>
            <ReactFlow nodes={nodes} edges={edges} onNodesChange={onNodesChange}
                       onEdgesChange={onEdgesChange} onConnect={onConnect}>
                <MiniMap/>
                <Background variant={BackgroundVariant.Dots} gap={12} size={1}/>
            </ReactFlow>
        </div>
    );
};

export default App;